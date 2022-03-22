import { ref, Ref, UnwrapRef } from "vue";

export default async function useFetch<T>(
  url: string
): Promise<{ data: Ref<UnwrapRef<T> | null>; error: Ref<string | null> }> {
  const data = ref<T | null>(null);
  const error = ref<string | null>(null);

  try {
    const response = await fetch(url);
    data.value = await response.json();
  } catch (e) {
    error.value = e as string;
  }

  return { data, error };
}
