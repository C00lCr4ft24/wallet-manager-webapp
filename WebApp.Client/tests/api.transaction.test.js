const { api, registerUser, loginUser, logResponse } = require('./setup')

const emails = ["tran1@transaction.com", "tran2@transaction.com", "t3@transaction.com"]
const passwords = ["pw1", "pw2", "pw3"]
let headerWithCookie = ""

let walletId = null
let transactionId = null

function toDateOnly(date) {
  return date.toISOString().split('T')[0]
}

describe('Transaction API Tests', () => {
  beforeAll(async () => {
    await registerUser(emails[0], passwords[0])
    await registerUser(emails[1], passwords[1])
    await registerUser(emails[2], passwords[2]) 
    headerWithCookie = await loginUser(emails[0], passwords[0])
  })
  
  test('Create Wallet', async () => {
    const response = await api.post('/wallet', 
      { name: "Test Wallet", balance: 5000 }, 
      headerWithCookie
    )
    walletId = response.data.id
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Create Transaction - Success', async () => {
    const response = await api.post(`/wallet/${walletId}/transaction`, 
      { walletId: walletId, date: toDateOnly(new Date()), name: "Test Transaction", amount: 1000, description: "Test Transaction", categoryId: 3 }, 
      headerWithCookie
    )
    transactionId = response.data.id
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Get Transaction - Success', async () => {
    const response = await api.get(`/transaction/${transactionId}`, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(200)
  })
  
  test('Create Transaction - Invalid Wallet', async () => {
    const response = await api.post(`/wallet/999/transaction`, 
      { walletId: 999, date: toDateOnly(new Date()), name: "Test Transaction", amount: 1000, description: "Test Transaction", categoryId: 3 }, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(404)
    })
  
  test('Create Transaction - Invalid Name', async () => {
    const response = await api.post(`/wallet/${walletId}/transaction`, 
      { walletId: walletId, date: toDateOnly(new Date()), name: "", amount: 1000, description: "Test Transaction", categoryId: 3 }, 
      headerWithCookie
    )
  logResponse(response)
    expect(response.status).toBe(400)
  })

  test('Create Transaction - Invalid Category', async () => {
    const response = await api.post(`/wallet/${walletId}/transaction`, 
      { walletId: walletId, date: toDateOnly(new Date()), name: "Test Transaction", amount: 1000, description: "Test Transaction", categoryId: 999 }, 
      headerWithCookie
    )
  logResponse(response)
    expect(response.status).toBe(404)
  })
})