import vue from '@vitejs/plugin-vue'

import createAutoImport from './auto-import'
import createSvgIcon from './svg-icon'
import createCompression from './compression'
import createSetupExtend from './setup-extend'
import { createStyleImportPlugin, VxeTableResolve } from 'vite-plugin-style-import'
import inject from '@rollup/plugin-inject'

export default function createVitePlugins(viteEnv, isBuild = false) {
  const vitePlugins = [
    vue(),
    inject({
      $: 'jquery', // 这里会自动载入 node_modules 中的 jquery
      jQuery: 'jquery',
      'window.jQuery': 'jquery'
    })
  ]
  vitePlugins.push(createAutoImport())
  vitePlugins.push(createSetupExtend())
  vitePlugins.push(createSvgIcon(isBuild))
  isBuild && vitePlugins.push(...createCompression(viteEnv))

  vitePlugins.push(
    createStyleImportPlugin({
      resolves: [VxeTableResolve()]
    })
  )
  return vitePlugins
}
