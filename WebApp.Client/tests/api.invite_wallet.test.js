const { api, registerUser, loginUser, logResponse } = require('./setup')

const emails = ["inv1@walletinv.com", "inv2@walletinv.com", "inv3@walletinv.com"]
const passwords = ["pw1", "pw2", "pw3"]
let headerWithCookie = ""

let familyInviteId = null
let newWalletId = null
let walletInviteId = null
let transaction = null

function toDateOnly(date) {
  return date.toISOString().split('T')[0]
}

describe('Wallet Invite API Tests', () => {
  beforeAll(async () => {
    await registerUser(emails[0], passwords[0])
    await registerUser(emails[1], passwords[1])
    await registerUser(emails[2], passwords[2]) 
    headerWithCookie = await loginUser(emails[0], passwords[0])
  })
  
  test('Create Wallet', async () => {
    const response = await api.post('/wallet', { name: "Test Wallet", balance: 5000 }, headerWithCookie)
    newWalletId = response.data.id
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Create Family Invite - Success', async () => {
    const response = await api.post('family/invite/send', { userEmail: emails[1] }, headerWithCookie)
    familyInviteId = response.data.id
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Accept Family Invite - Success', async () => {
    headerWithCookie = await loginUser(emails[1], passwords[1])
    const response = await api.post('family/invite/respond', { id: familyInviteId, accept : true }, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Create Invite - User does not exist', async () => {
    headerWithCookie = await loginUser(emails[0], passwords[0])
    const response = await api.post(`wallet/${newWalletId}/invite/send`, { userEmail: 'doesnotexist@doesnotexist.com' }, headerWithCookie )
    logResponse(response)
    expect(response.status).toBe(404)
  })
  test('Create Invite - Invalid Email Format', async () => {
    const response = await api.post(`wallet/${newWalletId}/invite/send`, { userEmail: 'this-is-not-a-valid-email.com' }, headerWithCookie )
    logResponse(response)
    expect(response.status).toBe(400)
  })

  test('Create Invite - Success', async () => {
    const response = await api.post(`wallet/${newWalletId}/invite/send`, { userEmail: emails[1] }, headerWithCookie )
    walletInviteId = response.data.id
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Create Invite - Already Sent', async () => {
    const response = await api.post(`wallet/${newWalletId}/invite/send`, { userEmail: emails[1] }, headerWithCookie )
    logResponse(response)
    expect(response.status).toBe(409)
  })


  test('Get Sent Invites', async () => {
    const response = await api.get(`wallet/${newWalletId}/invite/sent`, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Get Received Invites', async () => {
    headerWithCookie = await loginUser(emails[1], passwords[1])
    const response = await api.get(`wallet/${newWalletId}/invite/received`, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Accept Invite - Forbidden', async () => {
    headerWithCookie = await loginUser(emails[0], passwords[0])
    const response = await api.post(`wallet/invite/respond`, { id: walletInviteId, accept : true }, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(403)
  })

  test('Accept Invite - Success', async () => {
    headerWithCookie = await loginUser(emails[1], passwords[1])
    const response = await api.post('wallet/invite/respond', { id: walletInviteId, accept : true }, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Accept Invite - Already Responded', async () => {
    const response = await api.post('wallet/invite/respond', { id: walletInviteId, accept : true }, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(409)
  })

  test('Get Wallet After Accepting Invite', async () => {
    const response = await api.get(`/wallet/${newWalletId}?loadTransactions=true&loadUsers=true`, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Create Transaction with New User', async () => {
    const response = await api.post(`/wallet/${newWalletId}/transaction`,
       { walletId: newWalletId, date: toDateOnly(new Date()), name: "Test Transaction", amount: 1000, description: "Test Transaction", categoryId: 3 },
        headerWithCookie
      )
    transaction = response.data
    logResponse(response)
    expect(response.status).toBe(200)
  })
  test('Get Transaction After Creating Transaction with New User', async () => {
    const response = await api.get(`/transaction/${transaction.id}`, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(200)
  })
  test('Create Transaction with New User - Insufficient Permissions', async () => {
    headerWithCookie = await loginUser(emails[2], passwords[2])
    const response = await api.post(`/wallet/${newWalletId}/transaction`,
       { walletId: newWalletId, date: toDateOnly(new Date()), name: "Test Transaction 2", amount: 500, description: "Test Transaction 2", categoryId: 3 }, 
       headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(403)
  })
  test('Get Transaction After Creating Transaction with New User - Insufficient Permissions', async () => {
    const response = await api.get(`/transaction/1`, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(403)
  })
})