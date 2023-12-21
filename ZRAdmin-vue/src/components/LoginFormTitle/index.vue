<template>
  <div class="title-container">
    <img class="logo" src="/favicon.ico" />
    <div class="title">
      <!--      <span class="white"></span>-->
      <span class="blue">&nbsp;{{ title }}&nbsp;</span>
    </div>
    <el-tag class="tag" type="info" effect="plain" disable-transitions>BETA</el-tag>
  </div>
</template>
<script setup name="LoginFormTitle" lang="ts">
const title = ref(import.meta.env.VITE_APP_TITLE)
</script>
<style rel="stylesheet/scss" lang="scss" scoped>
.title-container {
  height: 90px;
  vertical-align: bottom;

  .logo {
    height: 50px;
    cursor: pointer;
    float: left;
  }

  .title {
    font-size: 26px;
    font-weight: 700;
    width: 400px;
    line-height: 50px;
    float: left;
  }

  .title .white {
    color: rgba(255, 255, 255, 1);
    font-size: 32px;
  }

  .title .blue {
    color: rgba(64, 158, 255, 1);
    font-size: 42px;
  }

  .tag {
    margin: 5px 0;
  }
}
</style>
