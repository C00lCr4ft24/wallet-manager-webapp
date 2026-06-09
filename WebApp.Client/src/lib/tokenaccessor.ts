export const setBearerToken = (token: string) => {
    localStorage.setItem('bearerToken', token)
}

export const getBearerToken = () => {
    return localStorage.getItem('bearerToken')
}

export const clearBearerToken = () => {
    localStorage.removeItem('bearerToken')
}