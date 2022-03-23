import useFetch, { IResult } from "@/composables/Fetch";
import IPackage from "@/models/IPackage";

let origin = `${document.location.origin}`;
if (process.env.NODE_ENV === "development") {
  origin = "http://localhost:4999";
}

export async function useSearch(): Promise<IResult<IPackage[]>> {
  const url = `${origin}/Templates/online`;
  return await useFetch<Array<IPackage>>(url);
}

export async function useInstalled(): Promise<IResult<IPackage[]>> {
  const url = `${origin}/Templates/installed`;
  return await useFetch<Array<IPackage>>(url);
}

export async function useInstall(id: string) {
  const url = `${origin}/Templates/installed/${id}`;
  return await useFetch<string>(url, "POST");
}

export async function useUninstall(id: string) {
  const url = `${origin}/Templates/installed/${id}`;
  return await useFetch<string>(url, "DELETE");
}
