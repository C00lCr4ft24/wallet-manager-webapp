const api = require('./config')

async function resetDatabase() {
  const response = await api.post('/reset-database/supersecret')
  return response.status === 200
}

async function registerUser(email, password) {
  const response = await api.post('/user/register-user', { email, password })
  return response.data
}

async function loginUser(email, password) {
  await logoutUser()
  const response = await api.post('/user/login?useCookies=true', { email, password })
  const authCookie = response.headers['set-cookie']
  return { headers: { 'Cookie': authCookie } }
}

async function logoutUser(headers) {
  if(!headers) { return }
  const response = await api.post('/user/logout', {}, headers)
  return response.status === 200
}

function logResponse(response) {
  console.log(`
    ##########################################################################################################################
    Status: ${response.status}
    Data: ${JSON.stringify(response.data)}
    ##########################################################################################################################
    ----------------------------------------------------------------------------------------------------------------------------------------------------
  `)
}

module.exports = {
  resetDatabase,
  registerUser,
  loginUser,
  logoutUser,
  logResponse,
  api
}