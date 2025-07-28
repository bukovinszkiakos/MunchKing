const BASE_URL = "";

interface ApiError {
  status: number;
  message: string;
}

async function apiFetch<T>(endpoint: string, method = "GET", data?: any): Promise<T> {
  const url = `${BASE_URL}${endpoint}`;
  const isFormData = data instanceof FormData;

  const headers: HeadersInit = {};
  if (!isFormData) {
    headers["Content-Type"] = "application/json";
  }

  const options: RequestInit = {
    method,
    headers,
    credentials: "include",
  };

  if (data) {
    options.body = isFormData ? data : JSON.stringify(data);
  }

  const response = await fetch(url, options);

  if (!response.ok) {
    if (response.status === 401) return null as T;

    let message = "Unknown error";
    try {
      const contentType = response.headers.get("content-type") || "";

      if (contentType.includes("application/json")) {
        const errorData = await response.json();
        if (errorData && typeof errorData === "object" && errorData.message) {
          message = errorData.message;
        } else {
          message = JSON.stringify(errorData);
        }
      } else {
        message = await response.text();
      }
    } catch {
      message = response.statusText;
    }

    throw {
      status: response.status,
      message,
    } as ApiError;
  }

  if (response.status === 204) return null as T;

  const contentType = response.headers.get("content-type") || "";
  const contentLength = response.headers.get("content-length");

  if (contentType.includes("application/json")) {
    try {
      return await response.json();
    } catch {
      return null as T;
    }
  }

  if (contentLength === "0") return null as T;

  return null as T;
}


export const apiGet = <T>(endpoint: string) => apiFetch<T>(endpoint, "GET");
export const apiPost = <T>(endpoint: string, data: any) => apiFetch<T>(endpoint, "POST", data);
export const apiPut = <T>(endpoint: string, data: any) => apiFetch<T>(endpoint, "PUT", data);
export const apiDelete = <T>(endpoint: string) => apiFetch<T>(endpoint, "DELETE");
