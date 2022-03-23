import { ref, Ref, UnwrapRef } from "vue";

export interface IResult<T> {
  readonly data: Ref<UnwrapRef<T> | null>;
  readonly error: Ref<string | null>;
}

export default async function useFetch<T>(
  url: string,
  method = "GET"
): Promise<IResult<T>> {
  const data = ref<T | null>(null);
  const error = ref<string | null>(null);

  try {
    const response = await fetch(url, { method });
    data.value = await response.json();
  } catch (e) {
    error.value = e as string;
  }

  return { data, error };
}
