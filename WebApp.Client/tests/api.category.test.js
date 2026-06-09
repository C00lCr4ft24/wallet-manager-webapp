const { api, registerUser, loginUser, logResponse } = require('./setup')

const emails = ["cat1@category.com", "cat2@category.com", "cat3@category.com"]
const passwords = ["pw1", "pw2", "pw3"]

let headerWithCookie = ""

const categoryIds = []
let newCategory = null

describe('Category API Tests', () => {
  beforeAll(async () => {
    await registerUser(emails[0], passwords[0])
    await registerUser(emails[1], passwords[1])
    await registerUser(emails[2], passwords[2])
    headerWithCookie = await loginUser(emails[0], passwords[0])
  })

  test('Create Category', async () => {
    const response = await api.post('/category', { name: "Test Category", description: "Test Category Description" }, headerWithCookie )
    logResponse(response)
    categoryIds.push(response.data.id)
    newCategory = response.data
    expect(response.status).toBe(200)
   })

   test('Get One Category - Success', async () => {
    const response = await api.get(`/category/${categoryIds[0]}`, headerWithCookie)
    logResponse(response)
    expect(response.data).toStrictEqual(newCategory)
    expect(response.status).toBe(200)
  })

  test('Update Category - Invalid Name', async () => {
    const response = await api.put(`/category/${categoryIds[0]}`, { name: "" }, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(400)
  })
  test('Update Category - Invalid Description', async () => {
    const response = await api.put(`/category/${categoryIds[0]}`, { description: "" }, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(400)
  })
  test('Update Category - Invalid Icon', async () => {
    const response = await api.put(`/category/${categoryIds[0]}`, { icon: "" }, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(400)
  })
  test('Update Category - Invalid Color', async () => {
    const response = await api.put(`/category/${categoryIds[0]}`, { color: "#invalid" }, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(400)
  })

  test('Get One Category - Not Found', async () => {
    const response = await api.get(`/category/999`, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(404)
  })

  test('Get New Category - Forbidden', async () => {
    headerWithCookie = await loginUser(emails[1], passwords[1])
    const response = await api.get(`/category/${categoryIds[0]}`, headerWithCookie)
    logResponse(response)
    expect(response.status).toBe(403)
  })
})