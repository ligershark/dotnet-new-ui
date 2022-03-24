import { IResult, useEmptyFetch, useFetch } from "@/composables/Fetch";
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
  const url = `${origin}/Packages/${id}`;
  return await useEmptyFetch(url, "POST");
}

export async function usePackageUninstall(id: string) {
  const url = `${origin}/Packages/${id}`;
  return await useEmptyFetch(url, "DELETE");
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
) {
  const url = new URL(`${origin}/Templates/${shortName}`);
  url.searchParams.append("outputPath", outputPath);
  if (name) {
    url.searchParams.append("name", name);
  }
  if (language) {
    url.searchParams.append("language", language);
  }

  console.log(language, url.toString());
  return await useEmptyFetch(url.toString(), "POST");
}
