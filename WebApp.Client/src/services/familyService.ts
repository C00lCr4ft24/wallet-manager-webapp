import type { CreateFamilyInviteDto, FamilyUserDto, RespondToInviteDto } from "@/generated/dtos";
import createFetch from "@/lib/fetchwrapper";

async function GetFamilyUsers() {
    const response = await createFetch('/api/family/users', 'GET')
    return response as FamilyUserDto[]
}

async function GetCurrentFamilyUser() {
    const response = await createFetch('/api/family/user', 'GET')
    return response as FamilyUserDto
}

async function GetFamilyUser(familyUserId: string) {
    const response = await createFetch(`/api/family/user/${familyUserId}`, 'GET')
    return response as FamilyUserDto
}

async function UpdateFamilyUser(familyUserId: string, isOwner: boolean) {
    const response = await createFetch(`/api/family/user/${familyUserId}?isOwner=${isOwner}`, 'PUT')
    return response as FamilyUserDto
}

async function SendInvite(request: CreateFamilyInviteDto) {
    const response = await createFetch('/api/family/invite/send', 'POST', request)
    return response
}

async function GetSentInvites() {
    const response = await createFetch('/api/family/invite/sent', 'GET')
    return response
}

async function GetReceivedInvites() {
    const response = await createFetch('/api/family/invite/received', 'GET')
    return response
}

async function RespondToInvite(request: RespondToInviteDto) {
    const response = await createFetch('/api/family/invite/respond', 'POST', request)
    return response
}

async function DeleteInvite(inviteId: number) {
    const response = await createFetch(`/api/family/invite/delete/${inviteId}`, 'DELETE')
    return response
}

const familyService = {
    GetFamilyUsers,
    GetCurrentFamilyUser,
    GetFamilyUser,
    SendInvite,
    GetSentInvites,
    GetReceivedInvites,
    RespondToInvite,
    UpdateFamilyUser,
    DeleteInvite
}

export default familyService