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
                const antiForgeryResponse = await resilientFetch("api/account/token", { method: "GET" });
                if (antiForgeryResponse.ok) {
                    const xsrfSection = document.cookie.split("; ").find(row => row.startsWith("XSRF-TOKEN="));
                    if (xsrfSection) {
                        xsrfToken = xsrfSection.split("=")[1];
                    }
                }
                else {
                    console.error("Error fetching token.")
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
        const response = await resilientFetch(url, request);
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

let cumulativeErrors = 0;
async function resilientFetch(input: RequestInfo | URL, request?: RequestInit): Promise<Response> {
    let retryAttempt = 0;
    let errorStatus = 500;
    let errorMessage = "Could not connect to services, please try again later.";
    while (retryAttempt < 3) {
        if (cumulativeErrors > 3) {
            return new Response("Could not connect to services, please try again later.", { status: 500 });
        }
        try {
            const response = await fetch(input, request);
            if (response.status > 501) {
                errorStatus = response.status;
                errorMessage = await response.text();
            }
            else {
                return response;
            }
        } catch (e) {
            if (e instanceof Error) {
                errorMessage = e.message;
            }
        }
        cumulativeErrors++;
        setTimeout(() => cumulativeErrors--, 10000);
        retryAttempt++;
        if (retryAttempt < 3) {
            await sleep(Math.pow(retryAttempt, 2) * 1000);
        }
    }
    return new Response(errorMessage, { status: errorStatus });
}

function sleep(delay: number) {
    return new Promise((resolve) => setTimeout(resolve, delay));
}