<template>
  <starBackground />
  <div class="login">
    <div class="login-wrapper">
      <LoginFormTitle />
      <el-form ref="loginRef" :model="loginForm" :rules="loginRules" class="login-form">
        <div class="link-wrapper">
          <LangSelect title="多语言设置" class="langSet" />
          <!--          <el-text type="primary" style="margin: 0 10px">-->
          <!--            <router-link :to="'/register'">{{ $t('login.register') }}</router-link>-->
          <!--          </el-text>-->
          <el-text type="primary">
            <router-link :to="'/login'">{{ $t('loginByEmail.toLogin') }}</router-link>
          </el-text>
        </div>
        <el-divider style="margin: 12px 0" />
        <el-form-item prop="email">
          <el-input v-model="loginForm.email" type="text" size="large" auto-complete="off" :placeholder="$t('loginByEmail.email')">
            <template #prefix>
              <svg-icon name="email" class="el-input__icon input-icon" />
            </template>
          </el-input>
        </el-form-item>
        <el-form-item prop="code">
          <!-- style="width: 66%" -->
          <el-input v-model="loginForm.code" size="large" auto-complete="off" :placeholder="$t('login.captcha')" @keyup.enter="handleLogin">
            <template #prefix>
              <svg-icon name="validCode" class="el-input__icon input-icon" />
            </template>
            <template #append>
              <el-button @click="getDragVerify" :loading="checkDragVerify.loading">
                {{ checkDragVerify.text }}
              </el-button>
            </template>
          </el-input>
          <!-- <el-button class="login-code-email" color="#626aef" size="large" @click="getDragVerify" :loading="checkDragVerify.loading">
                {{ checkDragVerify.text }}
              </el-button> -->
        </el-form-item>
        <el-form-item>
          <el-button :loading="loading" size="large" type="primary" style="width: 100%" @click.prevent="handleLogin">
            <span v-if="!loading">{{ $t('login.btnLogin') }}</span>
            <span v-else>{{ $t('login.btnLoginLoading') }}</span>
          </el-button>
        </el-form-item>
      </el-form>
    </div>
    <div class="el-login-footer">
      <div v-html="defaultSettings.copyright"></div>
    </div>
    <!-- 滑块验证 -->
    <el-dialog v-model="dragVerify" destroy-on-close width="440px" top="30vh">
      <DragVerifyImgChip
        :imgsrc="verifyImg.src"
        v-model:isPassing="isPassing"
        text="请按住滑块拖动"
        successText="验证通过"
        :width="400"
        :showRefresh="true"
        @refresh="refreshImg"
        @passcallback="verifyPass"
        style="border: 1px solid rgb(238, 238, 238)">
      </DragVerifyImgChip>
    </el-dialog>
  </div>
</template>

<script setup name="loginByEmail" lang="ts">
// import { getMailCode, getEmailVerifyImg, getRsaKey } from '@/api/system/login'
import defaultSettings from '@/settings'
import starBackground from '@/views/components/starBackground.vue'
import LangSelect from '@/components/LangSelect/index.vue'
import LoginFormTitle from '@/components/LoginFormTitle/index.vue'
import DragVerifyImgChip from '@/components/DragVerifyImgChip/index.vue'
import { useI18n } from 'vue-i18n'
import useCurrentInstance from '@/utils/useCurrentInstance'
import userUserStore from '@/store/modules/user'
import { encryptByPublicKey } from '@/utils/jsencrypt'
const { globalProperties } = useCurrentInstance()
const userStore = userUserStore()
const router = useRouter()
const route = useRoute()

interface checkDragVerify {
  loading: boolean
  duration: number
  timer: any
  text: string
}
interface loginForm {
  email: string
  code?: string
  uuid?: string
}

const { t } = useI18n()
const seconds = 30
const checkEmail = (_rule, value, cb) => {
  //验证邮箱的正则表达式
  const regEmail = /^\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$/
  if (regEmail.test(value)) {
    //合法的邮箱
    return cb()
  }
  cb(new Error('请输入合法的邮箱'))
}
const loginRules = {
  code: [{ required: true, trigger: 'change', message: t('loginByEmail.loginRules.dragVerifyRequired') }],
  email: [
    { required: true, trigger: 'blur', message: t('loginByEmail.loginRules.emailRequired') },
    { validator: checkEmail, trigger: 'blur', message: t('loginByEmail.loginRules.emailFormat') }
  ]
}

const loginRef = ref()
const loading = ref<boolean>(false)
const dragVerify = ref(false)
// const verifyImg = ref('https://picsum.photos/979/547' + '/?image=' + Math.round(Math.random() * 20))
// const /v.jpg
const isPassing = ref(false)
const loginForm = ref<loginForm>({
  email: '',
  code: '',
  uuid: ''
})
const checkDragVerify = reactive<checkDragVerify>({
  loading: false,
  duration: seconds,
  timer: null,
  text: t('loginByEmail.getDragVerify')
})
interface verifyImg {
  src: string
  array: Array<string>
  flag: number
}
const verifyImg: verifyImg = reactive({
  src: '',
  array: [],
  flag: 1
})
// 先存到数组中，然后刷新取下一个
function getVerifyImg() {
  // getEmailVerifyImg().then((res) => {
  //   verifyImg.array = res.data
  //   if (verifyImg.array.length === 0) {
  //     verifyImg.array.push('/v.jpg')
  //   }
  //   verifyImg.src = verifyImg.array[0]
  //   // resolve(verifyImg)
  // })
  verifyImg.array = [
    '/verifyimg/10-979x547.jpg',
    '/verifyimg/11-979x547.jpg',
    '/verifyimg/12-979x547.jpg',
    '/verifyimg/13-979x547.jpg',
    '/verifyimg/14-979x547.jpg',
    '/verifyimg/16-979x547.jpg',
    '/verifyimg/17-979x547.jpg',
    '/verifyimg/18-979x547.jpg',
    '/verifyimg/19-979x547.jpg',
    '/verifyimg/20-979x547.jpg',
    '/verifyimg/28-979x547.jpg',
    '/verifyimg/29-979x547.jpg',
    '/verifyimg/33-979x547.jpg',
    '/verifyimg/37-979x547.jpg'
  ]
  verifyImg.src = verifyImg.array[0]
}
//#region
// const verifyImgArrayFlag = ref(0)
// 目前问题，第一次为undefined，第二次为数组
// 第一次取数组o下标，第二次刷新再取1，到数组长度时，又重新取0
//#endregion
/**
 * @description: 刷新图片
 */
function refreshImg() {
  // 标量小于数组长度，直接取
  // 变量初始化为1，所以第一次取的时候，数组长度为1，所以会重新获取数组
  if (verifyImg.flag < verifyImg.array.length) {
    verifyImg.src = verifyImg.array[verifyImg.flag]
    verifyImg.flag++
  } else {
    verifyImg.flag = 0
    verifyImg.src = verifyImg.array[verifyImg.flag]
    verifyImg.flag++
  }
  // verifyImg.value = 'https://picsum.photos/979/547' + '/?image=' + Math.round(Math.random() * 20)
  // verifyImgArray.value.shift();
}
const redirect = ref()
redirect.value = route.query.redirect
interface roles {
  id: number
  name: string
}
interface LoginUser {
  createBy: string
  createTime: string
  delFlag: string
  userId: number
  userName: string
  nickName: string
  phonenumber?: string
  remark?: string
  email: string
  password: string
  sex: string
  status: string
  loginIp: string
  loginDate: string
  deptId: number
}
// const currentLoginUser = ref<LoginUser>()
let currentLoginCo: any[] = reactive([])
/**
 * @description: 登录
 */
function handleLogin() {
  loginRef.value.validate((valid) => {
    if (valid) {
      loading.value = true
      // 调用action的登录方法
      userStore
        .loginByEmail(loginForm.value)
        .then((res) => {
          if (res !== undefined && res.code === 300) {
            // 有多个角色，弹出选择角色的弹窗
            roleSelectDialog.visible = true
            const currentCoSysTypes = res.data.co.map((item) => item.coSysType)
            globalProperties.getDicts('co_sys_type').then((response) => {
              const roles: roles[] = []
              // 根据当前登录用户关联的企业类型，获取企业类型的字典信息
              currentCoSysTypes
                .map((item) => response.data.filter((i) => i.dictValue === item)[0])
                .forEach((element) => {
                  roles.push({
                    id: element.dictValue,
                    name: element.dictLabel
                  })
                  // currentLoginUser.value = res.data.user
                })
              roleSelectDialog.roles = roles
            })
            currentLoginCo = reactive(res.data.co)
            return
          }
          globalProperties.$modal.msgSuccess(globalProperties.$t('login.loginSuccess'))
          router.push({ path: redirect.value || '/' })
        })
        .catch((error) => {
          console.error(error)
          globalProperties.$modal.msgError(error.msg)
          loading.value = false
          // // 重新获取验证码
          // verifyPass()
        })
    }
  })
}
interface roleSelectDialog {
  visible: boolean
  roles: roles[]
}
const roleSelectDialog: roleSelectDialog = reactive({
  visible: false,
  roles: []
})
/**
 * @description: 企业类型选择弹窗关闭
 */
function roleSelectDialogClose(): void {
  loading.value = false
  loginForm.value.code = ''
  globalProperties.$modal.msg(t('roleSelectDialog.close'))
}
/**
 * @description: 企业类型选择弹窗回调
 * @param {roles} role 企业类型信息
 */
function roleSelectDialogCallback(role: roles): void {
  loading.value = false
  const { coSysType, coId } = currentLoginCo.find((item) => item.coSysType === role.id)
  userStore
    .loginByCoSysType({ coId: coId, cosysType: coSysType }) // , user: currentLoginUser.value
    .then(() => {
      router.push({ path: redirect.value || '/' })
    })
    .catch((error) => {
      console.error(error)
      globalProperties.$modal.msgError(error.msg)
      loading.value = false
      // 重新获取验证码
      // if (captchaOnOff.value) {
      //   getCode()
      //   loginForm.value.code = ''
      // }
    })
}
/**
 * @description: 获取滑动验证
 */
function getDragVerify() {
  loginRef.value.validateField('email', async (valid) => {
    if (valid) {
      isPassing.value = false
      dragVerify.value = true
    }
  })
}

/**
 * @description: 验证通过
 */
function verifyPass() {
  checkDragVerify.loading = true
  checkDragVerify.timer && clearInterval(checkDragVerify.timer)
  checkDragVerify.timer = setInterval(() => {
    const tmp = checkDragVerify.duration--
    checkDragVerify.text = `${tmp}` + t('loginByEmail.checkDragVerifyDuration')
    if (tmp <= 0) {
      clearInterval(checkDragVerify.timer)
      checkDragVerify.timer = null
      checkDragVerify.duration = seconds
      checkDragVerify.text = t('loginByEmail.getDragVerify')
      checkDragVerify.loading = false
    }
  }, 1000)
  getMailCode({ toUser: encryptByPublicKey(loginForm.value.email) }).then((res) => {
    // console.log(res)
  })
  setTimeout(() => {
    dragVerify.value = false
  }, 1500)
}
getVerifyImg()
</script>

<style lang="scss" scoped>
@import '@/assets/styles/login.scss';
</style>
