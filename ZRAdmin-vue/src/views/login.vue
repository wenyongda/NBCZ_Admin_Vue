<template>
  <starBackground />
  <div class="login">
    <div class="login-wrapper">
      <LoginFormTitle />
      <el-form ref="loginRef" :model="loginForm" :rules="loginRules" class="login-form">
        <div class="link-wrapper">
          <LangSelect title="多语言设置" class="langSet" />
          <el-text type="primary" style="margin: 0 10px">
            <router-link :to="'/register'">{{ $t('login.register') }}</router-link>
          </el-text>
          <!-- <el-text type="primary">
            <router-link :to="'/loginByEmail'">{{ $t('login.toLoginByEmail') }}</router-link>
          </el-text> -->
        </div>
        <el-divider style="margin: 12px 0" />
        <el-form-item prop="username">
          <el-input v-model="loginForm.username" type="text" size="large" auto-complete="on" :placeholder="$t('login.account')">
            <template #prefix>
              <svg-icon name="user" class="el-input__icon input-icon" />
            </template>
          </el-input>
        </el-form-item>
        <el-form-item prop="password">
          <el-input
            v-model="loginForm.password"
            type="password"
            size="large"
            auto-complete="off"
            :placeholder="$t('login.password')"
            @keyup.enter="handleLogin">
            <template #prefix>
              <svg-icon name="password" class="el-input__icon input-icon" />
            </template>
          </el-input>
        </el-form-item>
        <!-- <el-form-item prop="code" v-if="captchaOnOff != 'off'">
          <el-input
            v-model="loginForm.code"
            size="large"
            auto-complete="off"
            :placeholder="$t('login.captcha')"
            style="width: 66%"
            @keyup.enter="handleLogin">
            <template #prefix>
              <svg-icon name="validCode" class="el-input__icon input-icon" />
            </template>
          </el-input>
          <div class="login-code">
            <img :src="codeUrl" @click="getCode" class="login-code-img" />
          </div>
        </el-form-item> -->
        <div class="link-wrapper" style="text-align: left">
          <el-checkbox v-model="loginForm.rememberMe">{{ $t('login.rememberMe') }}</el-checkbox>
        </div>
        <el-form-item>
          <el-button :loading="loading" size="large" type="primary" style="width: 100%" @click.prevent="handleLogin">
            <span v-if="!loading">{{ $t('login.btnLogin') }}</span>
            <span v-else>{{ $t('login.btnLoginLoading') }}</span>
          </el-button>
        </el-form-item>
        <div class="other-login" v-if="defaultSettings.showOtherLogin">
          <el-divider>{{ $t('login.otherLoginWay') }}</el-divider>
          <span @click="onAuth('GITHUB')" title="github"><svg-icon name="github" className="login-icon"></svg-icon></span>
          <span @click="onAuth('GITEE')" title="gitee"><svg-icon name="gitee" className="login-icon"></svg-icon></span>
        </div>
      </el-form>
    </div>

    <!--  底部  -->
    <div class="el-login-footer">
      <div v-html="defaultSettings.copyright"></div>
    </div>
  </div>
</template>

<script setup name="login">
import { getCodeImg } from '@/api/system/login'
import Cookies from 'js-cookie'
import { encrypt, decrypt } from '@/utils/jsencrypt'
import defaultSettings from '@/settings'
import starBackground from '@/views/components/starBackground.vue'
import LangSelect from '@/components/LangSelect/index.vue'
import useUserStore from '@/store/modules/user'
import LoginFormTitle from '@/components/LoginFormTitle/index.vue'
const userStore = useUserStore()
const router = useRouter()
const route = useRoute()
const { proxy } = getCurrentInstance()

const loginForm = ref({
  username: '',
  password: '',
  rememberMe: false,
  code: '',
  uuid: ''
})
const host = window.location.host
const loginRules = {
  username: [{ required: true, trigger: 'blur', message: '请输入您的账号' }],
  password: [{ required: true, trigger: 'blur', message: '请输入您的密码' }],
  code: [{ required: true, trigger: 'change', message: '请输入验证码' }]
}

const codeUrl = ref('')
const loading = ref(false)
// 验证码开关
const captchaOnOff = ref('')
// 注册开关
const register = ref(false)
const redirect = ref()
redirect.value = route.query.redirect

function handleLogin() {
  proxy.$refs.loginRef.validate((valid) => {
    if (valid) {
      loading.value = true
      // 勾选了需要记住密码设置在cookie中设置记住用户名和密码
      if (loginForm.value.rememberMe) {
        Cookies.set('username', loginForm.value.username, { expires: 30, path: host })
        Cookies.set('password', encrypt(loginForm.value.password), { expires: 30, path: host })
        Cookies.set('rememberMe', loginForm.value.rememberMe, { expires: 30, path: host })
      } else {
        // 否则移除
        Cookies.remove('username')
        Cookies.remove('password')
        Cookies.remove('rememberMe')
      }
      // 调用action的登录方法
      userStore
        .login(loginForm.value)
        .then(() => {
          proxy.$modal.msgSuccess(proxy.$t('login.loginSuccess'))
          router.push({ path: redirect.value || '/' })
        })
        .catch((error) => {
          console.error(error)
          proxy.$modal.msgError(error.msg)
          loading.value = false
          // // 重新获取验证码
          // if (captchaOnOff.value) {
          //   getCode()
          // }
        })
    }
  })
}

function getCode() {
  getCodeImg().then((res) => {
    codeUrl.value = 'data:image/gif;base64,' + res.data.img
    loginForm.value.uuid = res.data.uuid
    captchaOnOff.value = res.data.captchaOff
  })
}

function getCookie() {
  const username = Cookies.get('username')
  const password = Cookies.get('password')
  const rememberMe = Cookies.get('rememberMe')
  loginForm.value = {
    username: username === undefined ? loginForm.value.username : username,
    password: password === undefined ? loginForm.value.password : decrypt(password),
    rememberMe: rememberMe === undefined ? false : Boolean(rememberMe)
  }
}
function onAuth(type) {
  userStore.setAuthSource(type)

  switch (type) {
    default:
      window.location.href = import.meta.env.VITE_APP_BASE_API + '/auth/Authorization?authSource=' + type
      break
  }
}
function handleForgetPwd() {
  proxy.$modal.msg('请联系管理员')
}
// getCode()
getCookie()
</script>

<style lang="scss" scoped>
@import '@/assets/styles/login.scss';
// .forget-pwd {
//   color: #ccc;
//   margin-right: 10px;
//   cursor: pointer;
// }
// .login-icon {
//   width: 30px;
//   height: 30px;
//   margin-right: 20px;
//   cursor: pointer;
// }
.other-login {
  padding: 0px 10px 5px;
}
</style>
