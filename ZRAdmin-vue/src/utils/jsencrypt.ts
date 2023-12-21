import JSEncrypt from 'jsencrypt/bin/jsencrypt.min'
import { KEYUTIL, KJUR } from 'jsrsasign'
// 密钥对生成 http://web.chacuo.net/netrsakeypair

const publicKey = 'MFwwDQYJKoZIhvcNAQEBBQADSwAwSAJBALj0zjON+EVdBsnMcR4Uj+jOYgp5ZipftQZ1utW8KvVioz+RSaotF1JHt59q9SC/mZcWWpbpcEqQ3WyyyCC33msCAwEAAQ=='

const privateKey =
  'MIIBVAIBADANBgkqhkiG9w0BAQEFAASCAT4wggE6AgEAAkEAuPTOM434RV0GycxxHhSP6M5iCnlmKl+1BnW61bwq9WKjP5FJqi0XUke3n2r1IL+ZlxZalulwSpDdbLLIILfeawIDAQABAkB5PYAtq1KjpWddwPYlkbUEFsWNuCaQgExZ/7KJiN9gGjo/UfUZ3W39Orb8PITIYf1NbasReqgddAcsfJNyoDWBAiEA7K89DyTmbjNSmekdD3rejRDdMzzXYtcbo69ZjHoowMUCIQDIDN8eg6PcWk4kiRcRYcNEfriUJR7Fg07ellSPv821bwIhAJA5TEyxIJUgQwI0cZfgOELfdtrlBR5ek6IPlNKsEa89AiBbMVroexPQWC41A3VLjChKagXUKpO7b98dIqRLnyCz6wIgP3qpvnO4IOxY7f5XarfCVyIHZJAMt/R1f16P5OkKv+A='

const publicPem = `
-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAo7yRx+lfHxy/c7lZFtZn
A3GBeQ+9nxBuOcSz/I8l0dP5JZsWVE+jMtmoxnFrTgCsnGuDHMNpSlexUc3IVcsp
bC4tSWh/r4Gy9KsIw0tIY0pulKsN/VhV//7dajMV/Dg+Rd5bwGGjWrlWOp2EPVYB
HI66uZ3PpEQe3FbEdaLkJRTENbywgGcGo1tqUpqY5BDGAu9IiXwZe4hdM2uniFS2
RKcYDovCiJE1lA7yyx9HEUd9PhYMhPMLvIIdPWW2Gqxsi3IZgG25hER0+o3zICgq
G/2VbodBM65JTLBY+KnY6H3o80b0v2qwOB7TktVtWQR6Lz8Ud+qrlgeAzvZb5Fmp
DQIDAQAB
-----END PUBLIC KEY-----`

// 加密
export function encrypt(txt) {
  const encryptor = new JSEncrypt()
  encryptor.setPublicKey(publicKey) // 设置公钥
  return encryptor.encrypt(txt) // 对数据进行加密
}

// 解密
export function decrypt(txt) {
  const encryptor = new JSEncrypt()
  encryptor.setPrivateKey(privateKey) // 设置私钥
  return encryptor.decrypt(txt) // 对数据进行解密
}

export const encryptByPublicKey = (txt: string, publicKey: string = publicPem) => {
  const pubKey = KEYUTIL.getKey(publicKey)
  // const ciphertext = KJUR.crypto.Cipher.encrypt(txt, pubKey, 'RSA')
  const ciphertext = pubKey.encrypt(txt)
  return btoa(atob(ciphertext).padStart(128, '\0'))
}
