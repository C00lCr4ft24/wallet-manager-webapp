import type { GetAllTransactionsDto, TransactionDataDto, UpdateTransactionDto } from "@/generated/dtos";
import createFetch from "@/lib/fetchwrapper";


async function getAllTransactions(request: GetAllTransactionsDto): Promise<TransactionDataDto[]> {
  const data = await createFetch('/api/transaction', 'POST', request)
  return data as TransactionDataDto[]
}

async function getTransactionById(id: number): Promise<TransactionDataDto> {
  const data = await createFetch(`/api/transaction/${id}`, 'GET')
  return data as TransactionDataDto
}

async function updateTransaction(id: number, data: UpdateTransactionDto): Promise<TransactionDataDto> {
  const updatedData = await createFetch(`/api/transaction/${id}`, 'PUT', data)
  return updatedData as TransactionDataDto
}

async function deleteTransaction(id: number): Promise<TransactionDataDto> {
  const deletedData = await createFetch(`/api/transaction/${id}`, 'DELETE')
  return deletedData as TransactionDataDto
}

const transactionService = {
  getAllTransactions,
  getTransactionById,
  updateTransaction,
  deleteTransaction
}

export default transactionService