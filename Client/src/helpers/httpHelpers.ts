export type HttpMethod = "GET" | "POST" | "PUT" | "PATCH" | "DELETE";

export async function send<T>(url: string, method: HttpMethod,
    body?: T | undefined, headers?: HeadersInit | undefined
): Promise<null | Error> {
    return await makeRequest<T, null>(url, method, undefined, body, headers);
}

export async function get<T, U>(url: string, method: HttpMethod, returnFunc: (response: Response) => Promise<U>,
    body?: T | undefined, headers?: HeadersInit | undefined
): Promise<U | Error> {
    return (await makeRequest<T, U>(url, method, returnFunc, body, headers)) as U | Error;
}

let xsrfToken = "";

async function makeRequest<T, U>(
    url: string,
    method: HttpMethod,
    returnFunc?: ((response: Response) => Promise<U>) | undefined,
    body?: T | undefined,
    headers?: HeadersInit | undefined
): Promise<U | Error | null> {
    try {
        const request: RequestInit = { method: method, headers: headers };
        if (method !== "GET") {
            if (!xsrfToken) {
                const antiForgeryResponse = await fetch("api/account/token", { method: "GET" });
                if (antiForgeryResponse.ok) {
                    const xsrfSection = document.cookie.split("; ").find(row => row.startsWith("XSRF-TOKEN="));
                    if (xsrfSection) {
                        xsrfToken = xsrfSection.split("=")[1];
                    }
                }
            }
            if (xsrfToken) {
                request.headers = { ...request.headers, "X-XSRF-TOKEN": xsrfToken };
            }
        }
        if (body) {
            request.body = JSON.stringify(body);
            request.headers = { ...request.headers, "Content-Type": "application/json" };
        }
        const response = await fetch(url, request);
        if (response.ok) {
            return returnFunc ? await returnFunc(response) : null;
        } else {
            return new Error(await response.text());
        }
    } catch (e) {
        if (e instanceof Error) {
            return e;
        }
        return new Error("Unknown error");
    }
}