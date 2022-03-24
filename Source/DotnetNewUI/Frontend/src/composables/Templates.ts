import useFetch, { IResult } from "@/composables/Fetch";
import IPackage from "@/models/IPackage";
import ITemplate from "@/models/ITemplate";

let origin = `${document.location.origin}`;
if (process.env.NODE_ENV === "development") {
  origin = "http://localhost:4999";
}

export async function usePackages(): Promise<IResult<IPackage[]>> {
  const url = `${origin}/Packages`;
  return await useFetch<Array<IPackage>>(url);
}

export async function usePackageInstall(id: string) {
  const url = `${origin}/Templates/installed/${id}`;
  return await useFetch<string>(url, "POST");
}

export async function usePackageUninstall(id: string) {
  const url = `${origin}/Templates/installed/${id}`;
  return await useFetch<string>(url, "DELETE");
}

export async function useTemplates(): Promise<IResult<ITemplate[]>> {
  const url = `${origin}/Templates`;
  return await useFetch<Array<ITemplate>>(url);
}

export async function useCreate(
  shortName: string,
  outputPath: string,
  name: string,
  language: string
): Promise<IResult<string>> {
  let url = `${origin}/Templates/${shortName}?outputPath=${outputPath}`;
  if (name) {
    url += `&name=${name}`;
  }
  if (language) {
    url += `&language=${language}`;
  }
  return await useFetch<string>(url, "POST");
}
