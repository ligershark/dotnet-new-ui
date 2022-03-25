import { ref } from "vue";
import NProgress from "vue-nprogress";

const isLoading = ref<boolean>(false);
const nprogress = new NProgress();

export default function useLoading() {
  function setIsLoading(value: boolean) {
    isLoading.value = value;
    if (isLoading.value) {
      nprogress.inc();
    } else {
      nprogress.done();
    }
  }

  return {
    setIsLoading,
    isLoading,
  };
}
