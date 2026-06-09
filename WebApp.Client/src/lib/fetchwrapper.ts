import { ProblemDetails } from "@/generated/dtos"

const getBearerToken = () => {
	return localStorage.getItem('bearerToken')
}

async function createFetch(url: string, method: string, body?: any) {
    const token = getBearerToken()
    const headers = {
        'Content-Type': 'application/json',
        ...(token && { 'Authorization': `Bearer ${token}` })
    }
    const response: Response = await fetch(url, { method, headers, body: JSON.stringify(body) })
    const data = await response.json()
    if (response.status === 200) {
        return data
    } else {
        throw ProblemDetails.fromJS(data)
    }
}

export default createFetch