const { api, registerUser, loginUser, logResponse } = require('./setup')

const emails = ["limit1@wallet.com", "limit2@wallet.com", "limit3@wallet.com"]
const passwords = ["pw1", "pw2", "pw3"]
const users = []
let headerWithCookie = ""

const walletIds = []
const limitIds = []

function toDateOnly(date) {
  return date.toISOString().split('T')[0]
}

describe('Limit API Tests', () => {
  beforeAll(async () => {
    const user0 = await registerUser(emails[0], passwords[0])
    const user1 = await registerUser(emails[1], passwords[1])
    const user2 = await registerUser(emails[2], passwords[2])
    users.push(user0, user1, user2)
    headerWithCookie = await loginUser(emails[0], passwords[0])
  })
  test('Create Wallet for User 0', async () => {
    const response = await api.post('/wallet', 
      { name: "Test Wallet", balance: 20000 }, 
      headerWithCookie
    )
    logResponse(response)
    walletIds.push(response.data.id)
    expect(response.status).toBe(200)
  })
  test('Create Limit - Invalid Amount', async () => {
    const response = await api.post('/limit',
      { userId: users[0].id, maxAmount: -100, startDate: toDateOnly(new Date()), endDate: toDateOnly(new Date(Date.now() + 86400000)), includeIncome: false, isActive: true }, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(400)
  })
  test('Create Limit - Invalid Start Date', async () => {
    const response = await api.post('/limit', 
      { userId: users[0].id, maxAmount: 100, startDate: '400', endDate: toDateOnly(new Date(Date.now() + 86400000)), includeIncome: false, isActive: true }, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(400)
  })
  test('Create Limit - Invalid End Date', async () => {
    const response = await api.post('/limit', 
      { userId: users[0].id, maxAmount: 100, startDate: toDateOnly(new Date()), endDate: '400', includeIncome: false, isActive: true }, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(400)
  })
  test('Create Limit - End Date Before Start Date', async () => {
    const response = await api.post('/limit', 
      { userId: users[0].id, maxAmount: 100, startDate: toDateOnly(new Date()), endDate: toDateOnly(new Date(Date.now() - 86400000)), includeIncome: false, isActive: true }, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(400)
  })

  test('Create Limit - Invalid Amount', async () => {
    const response = await api.post('/limit', 
      { userId: users[0].id, maxAmount: -100, startDate: toDateOnly(new Date()), endDate: toDateOnly(new Date(Date.now() + 86400000)), includeIncome: false, isActive: true }, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(400)
  })

  test('Create Limit as User 0 (Family Owner) for User 0', async () => {
    const response = await api.post('/limit', 
      { userId: users[0].id, maxAmount: 9900, startDate: toDateOnly(new Date()), endDate: toDateOnly(new Date(Date.now() + 86400000)), includeIncome: true, isActive: true }, 
      headerWithCookie
    )
    limitIds.push(response.data.id)
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Create Transaction that does not exceed Limit', async () => {
    let response = await api.post(`/wallet/${walletIds[0]}/transaction`, 
      { walletId: walletIds[0], date: new Date(), name: "Test Transaction", amount: -5000, description: "Test Transaction", categoryId: null }, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(200)
    response = await api.get(`/limit/${limitIds[0]}`, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(200)
    expect(response.data.currentAmount).toBe(5000)
  })
  test('Create Transaction that does exceeds Limit', async () => {
    let response = await api.post(`/wallet/${walletIds[0]}/transaction`, 
      { walletId: walletIds[0], date: new Date(), name: "Test Transaction", amount: -5000, description: "Test Transaction", categoryId: null }, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(400)
    response = await api.get(`/limit/${limitIds[0]}`, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(200)
    expect(response.data.currentAmount).toBe(5000)
  })

  test('Send and accept family invite for User 1', async () => {
    let response = await api.post('/family/invite/send', { userEmail: emails[1] }, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(200)
    const inviteId = response.data.id
    headerWithCookie = await loginUser(emails[1], passwords[1])
    response = await api.post('/family/invite/respond', { id: inviteId, accept: true }, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(200)
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Create Limit as User 1 (Not Family Owner) for User 0', async () => {
    headerWithCookie = await loginUser(emails[1], passwords[1])
    const response = await api.post('/limit', 
      { userId: users[0].id, maxAmount: 9900, startDate: toDateOnly(new Date()), endDate: toDateOnly(new Date(Date.now() + 86400000)), includeIncome: true, isActive: true }, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(403)
  })  

  test('Create Limit as User 0 (Family Owner) for User 1', async () => {
    headerWithCookie = await loginUser(emails[0], passwords[0])
    const response = await api.post('/limit', 
      { userId: users[1].id, maxAmount: 9900, startDate: toDateOnly(new Date()), endDate: toDateOnly(new Date(Date.now() + 86400000)), includeIncome: true, isActive: true }, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(200)
  })
})