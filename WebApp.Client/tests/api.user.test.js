const { api, registerUser, loginUser, logResponse } = require('./setup')

const emails = ["user1@user.com", "user2@user.com", "user3@user.com"]
const passwords = ["pw1", "pw2", "pw3"]
let headerWithCookie = ""

describe('User API Tests', () => {
  beforeAll(async () => {
    await registerUser(emails[0], passwords[0])
    await registerUser(emails[1], passwords[1])
    await registerUser(emails[2], passwords[2]) 
    headerWithCookie = await loginUser(emails[0], passwords[0])
  })

  test('Update User Email - Invalid Email Format', async () => {
    const response = await api.post('/user/update-email',
      { email: "this-is-not-a-valid-email.com" },
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(400)
  })

  test('Update User Email - Email Already in Use', async () => {
    const response = await api.post('/user/update-email', { email: emails[1] }, 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(409)
  })

  test('Update User Email - Success', async () => {
    const response = await api.post('/user/update-email',
      { email: "newemail@newemail.com" },
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Update User Password - Success', async () => {
    const response = await api.post('/user/update-password',
      { oldPassword: passwords[0], newPassword: "newpw1" },
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Update Username - Success', async () => {
    const response = await api.post('/user/update-username',
      { username: "newusername" },
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Update User Date of Birth - Success', async () => {
    const response = await api.post('/user/update-dob',
      { dateOfBirth: "1990-01-01" },
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(200)
  })

  test('Delete User - Success', async () => {
    const response = await api.delete('/user', 
      headerWithCookie
    )
    logResponse(response)
    expect(response.status).toBe(200)
  })
})