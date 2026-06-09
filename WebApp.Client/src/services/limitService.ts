import type { CreateLimitDto, LimitDto, UpdateLimitDto } from "@/generated/dtos"
import createFetch from "@/lib/fetchwrapper"

async function getLimitById(id: number): Promise<LimitDto> {
  const data = await createFetch(`/api/limit/${id}`, 'GET')
  return data as LimitDto
}

async function getAllLimits(userId?: number | undefined): Promise<LimitDto[]> {
  const data =
    userId !== undefined
      ? await createFetch(`/api/limit?userId=${userId}`, 'GET')
      : await createFetch(`/api/limit`, 'GET')
  return data as LimitDto[]
}

async function createLimit(request: CreateLimitDto): Promise<LimitDto> {
    const data = await createFetch(`/api/limit`, 'POST', request)
    return data as LimitDto
}

async function deleteLimit(id: number): Promise<LimitDto> {
    const data = await createFetch(`/api/limit/${id}`, 'DELETE')
    return data as LimitDto
}

async function updateLimit(id: number, request: UpdateLimitDto): Promise<LimitDto> {
    const data = await createFetch(`/api/limit/${id}`, 'PUT', request)
    return data as LimitDto
}

const limitService = {
    getLimitById,
    getAllLimits,
    createLimit,
    deleteLimit,
    updateLimit
}

export default limitService