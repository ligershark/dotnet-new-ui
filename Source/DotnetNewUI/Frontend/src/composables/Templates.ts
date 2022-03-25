import { IResult, useEmptyFetch, useFetch } from "@/composables/Fetch";
import IPackage from "@/models/IPackage";
import ITemplate from "@/models/ITemplate";
import useLoading from "./Loading";

let origin = `${document.location.origin}`;
if (process.env.NODE_ENV === "development") {
  origin = "http://localhost:4999";
}

export function getOriginUrl() {
  return origin;
}

export async function usePackages(): Promise<IResult<IPackage[]>> {
  const { setIsLoading } = useLoading();
  try {
    setIsLoading(true);
    const url = `${origin}/Packages`;
    return await useFetch<Array<IPackage>>(url);
  } finally {
    setIsLoading(false);
  }
}

export async function usePackageInstall(id: string) {
  const { setIsLoading } = useLoading();
  try {
    setIsLoading(true);
    const url = `${origin}/Packages/${id}`;
    return await useEmptyFetch(url, "POST");
  } finally {
    setIsLoading(false);
  }
}

export async function usePackageUninstall(id: string) {
  const { setIsLoading } = useLoading();
  try {
    setIsLoading(true);
    const url = `${origin}/Packages/${id}`;
    return await useEmptyFetch(url, "DELETE");
  } finally {
    setIsLoading(false);
  }
}

export async function useTemplates(): Promise<IResult<ITemplate[]>> {
  const { setIsLoading } = useLoading();
  try {
    setIsLoading(true);
    const url = `${origin}/Templates`;
    return await useFetch<Array<ITemplate>>(url);
  } finally {
    setIsLoading(false);
  }
}

export async function useCreate(
  shortName: string,
  outputPath: string,
  name: string,
  language: string
) {
  const { setIsLoading } = useLoading();
  try {
    setIsLoading(true);
    const url = new URL(`${origin}/Templates/${shortName}`);
    url.searchParams.append("outputPath", outputPath);
    if (name) {
      url.searchParams.append("name", name);
    }
    if (language) {
      url.searchParams.append("language", language);
    }

    return await useEmptyFetch(url.toString(), "POST");
  } finally {
    setIsLoading(false);
  }
}
