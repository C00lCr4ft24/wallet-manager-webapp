import { CategoryHeaderDto, CreateTransactionDto, ProblemDetails, TransactionDataDto, WalletDataDto } from "@/generated/dtos"
import walletService from "@/services/walletService"
import { useEffect, useState } from "react"
import { toast } from "sonner"
import { Dialog, DialogClose, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from "./ui/dialog"
import { Button } from "./ui/button"
import { Field, FieldGroup } from "./ui/field"
import { Input } from "./ui/input"
import { Label } from "./ui/label"
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from "./ui/select"
import categoryService from "@/services/categoryService"
import DateAndTImePicker from "./DateAndTimePicker"
import { getBgColorFromId } from "@/lib/utils"
import transactionService from "@/services/transactionService"

function TransactionDialog({ children, onTransactionAdded, transaction }: { children: React.ReactNode; onTransactionAdded: () => void; transaction?: TransactionDataDto }) {
  const [open, setOpen] = useState(false)

  const [name, setName] = useState<string>('')
  const [amount, setAmount] = useState<string>('')
  const [description, setDescription] = useState<string>('')
  const [date, setDate] = useState<Date>(new Date())

  const [categories, setCategories] = useState<CategoryHeaderDto[]>([])
  const [selectedCategory, setSelectedCategory] = useState<CategoryHeaderDto | null>(null)

  const [wallets, setWallets] = useState<WalletDataDto[]>([])
  const [selectedWallet, setSelectedWallet] = useState<WalletDataDto | null>(null)

  const isEditMode = !!transaction

  useEffect(() => {
    const fetchWallets = async () => {
      try {
        const w = await walletService.GetWallets(false, false)
        const c = await categoryService.GetCategories()
        setCategories(c)
        setWallets(w)
        if (transaction) {
          setName(transaction.name)
          setAmount(transaction.amount?.toString() ?? '')
          setDescription(transaction.description ?? '')
          setDate(new Date(transaction.date))
          const transactionCategory = c.find((cat: CategoryHeaderDto) => cat.id === transaction.category?.id) || null
          setSelectedCategory(transactionCategory)
          const transactionWallet = w.find((wal: WalletDataDto) => wal.id === transaction.wallet?.id) || null
          setSelectedWallet(transactionWallet)
        }
      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      }
    }
    fetchWallets()
  }, [])

  const handleDelete = async () => {
    if (!transaction) return

    try {
      await transactionService.deleteTransaction(transaction.id)
      toast.success("Transaction deleted successfully!", { position: 'bottom-center' })
      onTransactionAdded()
      setOpen(false)
    } catch (e) {
      if (e instanceof ProblemDetails) {
        toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
      }
    }
  }

  const handleSubmit = async () => {
    if (!name || !amount || !date || !selectedWallet) {
      toast.error("Name, amount, date and wallet are required!", { position: 'bottom-center' })
      return
    }
    try {
      if (isEditMode && transaction) {
        await transactionService.updateTransaction(
          transaction.id,
          new CreateTransactionDto({
            name: name,
            amount: parseFloat(amount) || 0,
            date: date,
            categoryId: selectedCategory?.id,
            walletId: selectedWallet.id!,
            description: description
          })
        )
        toast.success("Transaction updated successfully!", { position: 'bottom-center' })
      }
      if (!isEditMode) {
        await walletService.CreateTransaction(
          selectedWallet.id!,
          new CreateTransactionDto({
            name: name,
            amount: parseFloat(amount) || 0,
            date: date,
            categoryId: selectedCategory?.id,
            description: description
          })
        )
        toast.success("Transaction created successfully!", { position: 'bottom-center' })
      }
      onTransactionAdded()
      setOpen(false)
    } catch (e) {
      if (e instanceof ProblemDetails) {
        toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
      }
    }
  }
  return (
    <Dialog open={open} onOpenChange={(isOpen) => setOpen(isOpen)}>
      <form>
        <DialogTrigger asChild>
          {children}
        </DialogTrigger>
        <DialogContent className="sm:max-w-sm">
          <DialogHeader>
            <DialogTitle>Add Transaction</DialogTitle>
            <DialogDescription>
              Enter the details of the transaction you want to create.
            </DialogDescription>
          </DialogHeader>
          <FieldGroup>
            <Field>
              <Label htmlFor="name">Name*</Label>
              <Input id="name" name="name" type="text" value={name} onChange={(e) => setName(e.target.value)} onKeyDown={(e) => { if (e.key === 'Enter') { handleSubmit() } }} />
            </Field>
            <Field>
              <Label htmlFor="amount">Amount*</Label>
              <Input id="amount" name="amount" type="number" value={amount} onChange={(e) => setAmount(e.target.value)} onKeyDown={(e) => { if (e.key === 'Enter') { handleSubmit() } }} />
            </Field>
            <Field>
              <Label htmlFor="description">Description</Label>
              <Input id="description" name="description" type="text" value={description} onChange={(e) => setDescription(e.target.value)} />
            </Field>
            <Field>
              <Label htmlFor="date">Date*</Label>
              <DateAndTImePicker onDateTimeChange={setDate} />
            </Field>
            <Field>
              <Label htmlFor="wallet">Wallet*</Label>
              <Select value={selectedWallet ? selectedWallet.id?.toString() ?? '0' : ''} onValueChange={(value) => setSelectedWallet(wallets.find((w) => w.id === parseInt(value)) || null)}>
                <SelectTrigger id="wallet">
                  <SelectValue placeholder="Select a wallet" />
                </SelectTrigger>
                <SelectContent>
                  <SelectGroup>
                    {wallets.map((wallet) => (
                      <SelectItem key={wallet.id} value={wallet.id?.toString() ?? '0'} style={{ backgroundColor: getBgColorFromId(wallet.id!) + '20' }}>
                        {wallet.name}
                      </SelectItem>
                    ))}
                  </SelectGroup>
                </SelectContent>
              </Select>
            </Field>
            <Field>
              <Label htmlFor="category">Category</Label>
              <Select value={selectedCategory ? selectedCategory.id?.toString() ?? '0' : ''} onValueChange={(value) => setSelectedCategory(categories.find((c) => c.id === parseInt(value)) || null)}>
                <SelectTrigger id="category">
                  <SelectValue placeholder="Select a category" />
                </SelectTrigger>
                <SelectContent>
                  <SelectGroup>
                    {categories.map((category) => (
                      <SelectItem key={category.id} value={category.id?.toString() ?? '0'} style={{ backgroundColor: category.color ? category.color + '20' : 'transparent' }}>
                        {category.icon} {category.name}
                      </SelectItem>
                    ))}
                  </SelectGroup>
                </SelectContent>
              </Select>
            </Field>
          </FieldGroup>
          <DialogFooter>
            <DialogClose asChild>
              <Button variant="outline">Cancel</Button>
            </DialogClose>
            {isEditMode ? (
              <>
                <Button variant="destructive" type="submit" onClick={() => { handleDelete() }}>
                  Delete
                </Button>
                <Button type="submit" onClick={() => { handleSubmit() }}>
                  Save
                </Button>
              </>
            ) : (
              <Button type="submit" onClick={() => { handleSubmit() }}>
                Create
              </Button>
            )}
          </DialogFooter>
        </DialogContent>
      </form>
    </Dialog>
  )
}

export default TransactionDialog