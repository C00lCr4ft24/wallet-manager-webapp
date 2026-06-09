const { api, registerUser, loginUser, logResponse } = require('./setup')

const emails = ["fam1@family.com", "fam2@family.com", "fam3@family.com"]
const passwords = ["pw1", "pw2", "pw3"]

let headerWithCookie = ""
let inviteId = ""
describe('Family Invite API Tests', () => {
  beforeAll(async () => {
    await registerUser(emails[0], passwords[0])
    await registerUser(emails[1], passwords[1])
    headerWithCookie = await loginUser(emails[0], passwords[0])
  })
  
  test('Create Invite - User does not exist', async () => {
    const response = await api.post('family/invite/send', { userEmail: 'doesnotexist@doesnotexist.com' }, headerWithCookie )
    logResponse(response)
    expect(response.status).toBe(404)
  })
  test('Create Invite - Invalid Email Format', async () => {
    const response = await api.post('family/invite/send', { userEmail: 'this-is-not-a-valid-email.com' }, headerWithCookie )
    logResponse('Create Invite - Invalid Email Format', response)
    expect(response.status).toBe(400)
  })

  test('Create Invite - Success', async () => {
    const response = await api.post('family/invite/send', { userEmail: emails[1] }, headerWithCookie )
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Create Invite - Already Sent', async () => {
    const response = await api.post('family/invite/send', { userEmail: emails[1] }, headerWithCookie )
    logResponse(response)
    expect(response.status).toBe(409)
  })


  test('Get Sent Invites', async () => {
    const response = await api.get('family/invite/sent', headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Get Received Invites', async () => {
    headerWithCookie = await loginUser(emails[1], passwords[1])
    const response = await api.get('family/invite/received', headerWithCookie)
    inviteId = response.data[0].id
    //logResponse('Get Received Invites', response)
    expect(response.status).toBe(200)
  })

  test('Accept Invite - Forbidden', async () => {
    headerWithCookie = await loginUser(emails[0], passwords[0])
    const response = await api.post('family/invite/respond', { id: inviteId, accept : true }, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(403)
  })

  test('Accept Invite - Success', async () => {
    headerWithCookie = await loginUser(emails[1], passwords[1])
    const response = await api.post('family/invite/respond', { id: inviteId, accept : true }, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Accept Invite - Already Responded', async () => {
    const response = await api.post('family/invite/respond', { id: inviteId, accept : true }, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(409)
  })
})