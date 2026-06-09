const axios = require('axios')
const AxiosLogger = require('axios-logger')
const https = require('https')

const instance = axios.create({
  baseURL: 'https://localhost:5001',
  timeout: 300000,
  withCredentials: true,
  validateStatus: () => true,
  httpsAgent: new https.Agent({
    rejectUnauthorized: false
  })
})

instance.interceptors.request.use(AxiosLogger.requestLogger, AxiosLogger.errorLogger)
instance.interceptors.response.use(AxiosLogger.responseLogger, AxiosLogger.errorLogger)

module.exports = instance