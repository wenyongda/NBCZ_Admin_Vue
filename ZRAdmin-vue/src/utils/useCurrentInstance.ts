import { ComponentCustomProperties, ComponentInternalInstance, getCurrentInstance } from 'vue'
interface UseCurrentInstance {
  globalProperties: ComponentCustomProperties & Record<string, any>
}
export default function useCurrentInstance(): UseCurrentInstance {
  const { appContext } = getCurrentInstance() as ComponentInternalInstance
  const globalProperties = appContext.config.globalProperties
  return {
    globalProperties
  }
}
