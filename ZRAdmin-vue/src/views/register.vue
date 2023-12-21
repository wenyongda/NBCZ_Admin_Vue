<template>
  <starBackground></starBackground>
  <div class="login">
    <div class="login-wrapper">
      <LoginFormTitle />
      <el-form ref="registerFormRef" :model="registerForm" :rules="registerRules" class="login-form">
        <div class="link-wrapper">
          <LangSelect title="多语言设置" class="langSet" />
          <el-text type="primary">
            <router-link :to="'/login'">使用已有账户登录</router-link>
          </el-text>
        </div>
        <el-divider style="margin: 12px 0" />
        <el-form-item prop="username">
          <el-input v-model="registerForm.username" type="text" size="large" auto-complete="off" placeholder="账号">
            <template #prefix>
              <svg-icon name="user" />
            </template>
          </el-input>
        </el-form-item>
        <el-form-item prop="password">
          <el-input
            v-model="registerForm.password"
            type="password"
            size="large"
            auto-complete="off"
            placeholder="密码"
            @keyup.enter="handleRegister">
            <template #prefix>
              <svg-icon name="password" />
            </template>
          </el-input>
        </el-form-item>
        <el-form-item prop="confirmPassword">
          <el-input
            v-model="registerForm.confirmPassword"
            type="password"
            size="large"
            auto-complete="off"
            placeholder="确认密码"
            @keyup.enter="handleRegister">
            <template #prefix>
              <svg-icon name="password" />
            </template>
          </el-input>
        </el-form-item>
        <el-form-item prop="code" v-if="captchaOnOff">
          <el-input
            v-model="registerForm.code"
            size="large"
            auto-complete="off"
            placeholder="验证码"
            style="width: 66%"
            @keyup.enter="handleRegister">
            <template #prefix>
              <svg-icon name="validCode" />
            </template>
          </el-input>
          <div class="login-code ml10">
            <img :src="codeUrl" @click="getCode" class="login-code-img" />
          </div>
        </el-form-item>
        <el-form-item>
          <el-button :loading="loading" type="primary" size="large" style="width: 100%" @click.prevent="handleRegister">
            <span v-if="!loading">注 册</span>
            <span v-else>注 册 中...</span>
          </el-button>
        </el-form-item>
      </el-form>
    </div>
    <!--  底部  -->
    <div class="el-login-footer">
      <div v-html="defaultSettings.copyright"></div>
    </div>
  </div>
</template>

<script setup name="register" lang="ts">
import starBackground from '@/views/components/starBackground.vue'
import LangSelect from '@/components/LangSelect/index.vue'
import LoginFormTitle from '@/components/LoginFormTitle/index.vue'
import { getCodeImg, register } from '@/api/system/login'
import defaultSettings from '@/settings'
import { ElMessageBox } from 'element-plus'
const { proxy } = getCurrentInstance() as any
const router = useRouter()
const codeUrl = ref('')
const registerForm = reactive({
  username: '',
  password: '',
  confirmPassword: '',
  code: '',
  uuid: ''
})

const registerFormRef = ref(null)
const loading = ref(false)
const captchaOnOff = ref(true)
const equalToPassword = (_rule, value, callback) => {
  if (registerForm.password !== value) {
    callback(new Error('两次输入的密码不一致'))
  } else {
    callback()
  }
}
const registerRules = reactive({
  username: [
    { required: true, trigger: 'blur', message: '请输入您的账号' },
    {
      min: 2,
      max: 20,
      message: '用户账号长度必须介于 2 和 20 之间',
      trigger: 'blur'
    }
  ],
  password: [
    { required: true, trigger: 'blur', message: '请输入您的密码' },
    {
      min: 5,
      max: 20,
      message: '用户密码长度必须介于 6 和 20 之间',
      trigger: 'blur'
    }
  ],
  confirmPassword: [
    { required: true, trigger: 'blur', message: '请再次输入您的密码' },
    { required: true, validator: equalToPassword, trigger: 'blur' }
  ],
  code: [{ required: true, trigger: 'change', message: '请输入验证码' }]
})

function getCode() {
  getCodeImg().then((res) => {
    codeUrl.value = 'data:image/gif;base64,' + res.data.img
    registerForm.uuid = res.data.uuid
  })
}
function handleRegister() {
  proxy.$refs['registerFormRef'].validate((valid) => {
    if (valid) {
      loading.value = true
      register(registerForm)
        .then((res) => {
          if (res.code == 200) {
            const username = registerForm.username
            ElMessageBox.alert("<font color='red'>恭喜你，您的账号 " + username + ' 注册成功！</font>', '系统提示', {
              dangerouslyUseHTMLString: true,
              type: 'success'
            })
              .then(() => {
                router.push('/login')
              })
              .catch(() => {})
          }
        })
        .catch(() => {
          loading.value = false
          if (captchaOnOff.value) {
            getCode()
          }
        })
    }
  })
}
getCode()
</script>

<style rel="stylesheet/scss" lang="scss">
@import '@/assets/styles/login.scss';
</style>
