import type { CreateTransactionDto, CreateWalletDto, CreateWalletInviteDto, RespondToInviteDto, UpdateWalletDto, WalletDataDto } from "@/generated/dtos";
import createFetch from "@/lib/fetchwrapper";

async function CreateWallet(request: CreateWalletDto) {
    return await createFetch('/api/wallet', 'POST', request)
}

async function GetWallets(loadUsers: boolean, loadTransactions: boolean): Promise<WalletDataDto[]> {
    const params = new URLSearchParams()
    params.append('loadUsers', loadUsers.toString())
    params.append('loadTransactions', loadTransactions.toString())
    return await createFetch(`/api/wallet?${params}`, 'GET')
}

async function GetWallet(walletId: number, loadUsers: boolean, loadTransactions: boolean): Promise<WalletDataDto> {
    const params = new URLSearchParams()
    params.append('loadUsers', loadUsers.toString())
    params.append('loadTransactions', loadTransactions.toString())
    return await createFetch(`/api/wallet/${walletId}?${params}`, 'GET')
}

async function SendInvite(walletId: number, request: CreateWalletInviteDto) {
    const response = await createFetch(`/api/wallet/${walletId}/invite/send`, 'POST', request)
    return response
}

async function GetSentInvites(walletId: number) {
    const response = await createFetch(`/api/wallet/${walletId}/invite/sent`, 'GET')
    return response
}
async function GetAllSentInvites() {
    const response = await createFetch(`/api/wallet/invite/sent`, 'GET')
    return response
}

async function GetReceivedInvites(walletId: number) {
    return await createFetch(`/api/wallet/${walletId}/invite/received`, 'GET')
}
async function GetAllReceivedInvites() {
    return await createFetch(`/api/wallet/invite/received`, 'GET')
}

async function RespondToInvite(request: RespondToInviteDto) {
    return await createFetch('/api/wallet/invite/respond', 'POST', request)
}

async function DeleteInvite(walletId: number, inviteId: number) {
    return await createFetch(`/api/wallet/${walletId}/invite/${inviteId}`, 'DELETE')
}

async function CreateTransaction(walletId: number, request: CreateTransactionDto) {
    return await createFetch(`/api/wallet/${walletId}/transaction`, 'POST', request)

}

async function UpdateWalletUser(walletId: number, walletUserId: number, isOwner: boolean) {
    return await createFetch(`/api/wallet/${walletId}/user/${walletUserId}?isOwner=${isOwner}`, 'PUT')
}

async function GetWalletUser(walletUserId: number) {
    return await createFetch(`/api/wallet/user/${walletUserId}`, 'GET')
}

async function EditWallet(walletId: number, request: UpdateWalletDto) {
    return await createFetch(`/api/wallet/${walletId}`, 'PUT', request)
}

async function DeleteWallet(walletId: number) {
    return await createFetch(`/api/wallet/${walletId}`, 'DELETE')
}

const walletService = {
    CreateWallet,
    GetWallets,
    SendInvite,
    GetSentInvites,
    GetReceivedInvites,
    RespondToInvite,
    DeleteInvite,
    GetAllSentInvites,
    GetAllReceivedInvites,
    CreateTransaction,
    GetWallet,
    UpdateWalletUser,
    GetWalletUser,
    EditWallet,
    DeleteWallet
}

export default walletService