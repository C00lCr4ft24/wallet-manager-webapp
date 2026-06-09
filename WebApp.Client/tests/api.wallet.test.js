const { api, registerUser, loginUser, logResponse } = require('./setup')

const emails = ["wallet1@wallet.com", "wallet2@wallet.com", "wallet3@wallet.com"]
const passwords = ["pw1", "pw2", "pw3"]
const users = []
let headerWithCookie = ""

const walletIds = []

describe('Wallet API Tests', () => {
  beforeAll(async () => {
    const user1 = await registerUser(emails[0], passwords[0])
    const user2 = await registerUser(emails[1], passwords[1])
    const user3 = await registerUser(emails[2], passwords[2])
    users.push(user1, user2, user3)
    headerWithCookie = await loginUser(emails[0], passwords[0])
  })

  test('Create Wallet', async () => {
    const response = await api.post('/wallet', 
      { name: "Test Wallet", balance: 5000 }, 
      headerWithCookie
    )
    logResponse(response)
    walletIds.push(response.data.id)
    expect(response.status).toBe(200)
  })

  test('Create Wallet', async () => {
    const response = await api.post('/wallet', 
      { name: "Test Wallet 2", balance: 10000 }, 
      headerWithCookie
    )
    logResponse(response)
    walletIds.push(response.data.id)
    expect(response.status).toBe(200)
  })

  test('Get One Wallet', async () => {
    const response = await api.get(`/wallet/${walletIds[0]}?loadTransactions=true&loadUsers=true`, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Get All Wallets', async () => {
    const response = await api.get('/wallet?loadTransactions=true&loadUsers=true', 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Update Wallet', async () => {
    const response = await api.put(`/wallet/${walletIds[0]}`, 
      { name: "Updated Wallet Name", balance: 7500 }, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Trying to Update Wallet with Non-Owner', async () => {
    headerWithCookie = await loginUser(emails[1], passwords[1])
    const response = await api.put(`/wallet/${walletIds[0]}`, 
      { name: "Hacked Wallet Name", balance: 0 }, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(403)
  })

  test('Trying to Get Wallet with non-member', async () => {
    headerWithCookie = await loginUser(emails[2], passwords[2])
    const response = await api.get(`/wallet/${walletIds[0]}?loadTransactions=true&loadUsers=true`, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(403)
  })

  // Add more wallet-specific tests
})