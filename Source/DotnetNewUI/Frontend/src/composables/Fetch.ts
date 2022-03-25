import { ref, Ref, UnwrapRef } from "vue";
import useLoading from "./Loading";

export interface IEmptyResult {
  readonly error: Ref<string | null>;
}

export interface IResult<T> {
  readonly data: Ref<UnwrapRef<T> | null>;
  readonly error: Ref<string | null>;
}

export async function useEmptyFetch(
  url: string,
  method = "GET"
): Promise<IEmptyResult> {
  const error = ref<string | null>(null);
  const { setIsLoading } = useLoading();

  try {
    setIsLoading(true);
    await fetch(url, { method });
  } catch (e) {
    error.value = e as string;
  } finally {
    setIsLoading(false);
  }

  return { error };
}

export async function useFetch<T>(
  url: string,
  method = "GET"
): Promise<IResult<T>> {
  const data = ref<T | null>(null);
  const error = ref<string | null>(null);
  const { setIsLoading } = useLoading();

  try {
    setIsLoading(true);
    const response = await fetch(url, { method });
    data.value = await response.json();
  } catch (e) {
    error.value = e as string;
  } finally {
    setIsLoading(false);
  }

  return { data, error };
}
