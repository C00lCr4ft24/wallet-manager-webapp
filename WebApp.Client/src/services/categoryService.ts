import type { CreateCategoryDto, UpdateCategoryDto } from "@/generated/dtos";
import createFetch from "@/lib/fetchwrapper";

async function CreateCategory(request: CreateCategoryDto) {
    return await createFetch('/api/category', 'POST', request)
}

async function GetCategories() {
    return await createFetch('/api/category', 'GET')
}

async function GetCategory(id: number) {
    return await createFetch(`/api/category/${id}`, 'GET')
}

async function DeleteCategory(id: number) {
    return await createFetch(`/api/category/${id}`, 'DELETE')
}

async function UpdateCategory(id: number, data: UpdateCategoryDto) {
    return await createFetch(`/api/category/${id}`, 'PUT', data)
}

const categoryService = {
    CreateCategory,
    GetCategories,
    GetCategory,
    DeleteCategory,
    UpdateCategory
}

export default categoryService