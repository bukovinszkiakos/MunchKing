const BASE_URL = "http://localhost:5136";

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
      const errorData = await response.text();
      message = errorData || response.statusText;
    } catch {}

    throw {
      status: response.status,
      message,
    } as ApiError;
  }

  if (response.status === 204) return null as T;

  return response.json();
}

export const apiGet = <T>(endpoint: string) => apiFetch<T>(endpoint, "GET");
export const apiPost = <T>(endpoint: string, data: any) => apiFetch<T>(endpoint, "POST", data);
export const apiPut = <T>(endpoint: string, data: any) => apiFetch<T>(endpoint, "PUT", data);
export const apiDelete = <T>(endpoint: string) => apiFetch<T>(endpoint, "DELETE");
