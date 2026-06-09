import TransactionDialog from "@/components/TransactionDialog"
import CategoryLister from "@/components/CategoryLister"
import { Button } from "@/components/ui/button"
import WalletLister from "@/components/WalletLister"
import { useEffect, useState } from "react"
import {
  Table,
  TableBody,
  TableCell,
  TableFooter,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"
import CategoryItem from "@/components/CategoryItem"
import TransactionItem from "@/components/TransactionItem"
import WalletItem from "@/components/WalletItem"
import { Plus, CalendarIcon } from "lucide-react"
import { GetAllTransactionsDto, ProblemDetails, type TransactionDataDto } from "@/generated/dtos"
import transactionService from "@/services/transactionService"
import { toast } from "sonner"
import { formatDate } from "@/lib/utils"
import type { DateRange } from "react-day-picker"
import { addDays, format, subDays } from "date-fns"
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover"
import { Label } from "@/components/ui/label"
import { Calendar } from "@/components/ui/calendar"
import UserAvatarItem from "@/components/UserAvatarItem"

function TransactionsPage({ walletIdsToLoad }: { walletIdsToLoad?: number[] }) {
  const [refetchTrigger, setRefetchTrigger] = useState<number>(0)
  const [isLoading, setIsLoading] = useState<boolean>(true)

  const numOfColumns = 7

  const [walletIds, setWalletIds] = useState<number[]>(walletIdsToLoad && walletIdsToLoad.length > 0 ? walletIdsToLoad : [])
  const [categoryIds, setCategoryIds] = useState<number[]>([])
  const [transactions, setTransactions] = useState<TransactionDataDto[]>([])

  const normalizeDate = (d: Date) => {
    const normalized = new Date(d);
    normalized.setHours(0, 0, 0, 0);
    return normalized;
  }

  const [date, setDate] = useState<DateRange | undefined>({
    from: normalizeDate(subDays(new Date(), 31)),
    to: normalizeDate(new Date())
  })

  useEffect(() => {
    const fetchTransactions = async () => {
      try {
        let data = await transactionService.getAllTransactions(new GetAllTransactionsDto({
          categoryIds: categoryIds,
          walletIds: walletIds,
          startDate: subDays(date?.from!, 1),
          endDate: addDays(date?.to!, 1)
        }))
        data = data.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
        setTransactions(data)
        console.log(data)
      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      } finally {
        setIsLoading(false)
      }
    }
    fetchTransactions()
  }, [refetchTrigger])

  const formatAmount = (amount: number | undefined) => {
    if (amount === undefined) return "N/A"
    const sign = amount < 0 ? "-" : "+"
    return `${sign}$${Math.abs(amount).toFixed(2)}`
  }

  return (
    <div className="px-12 py-12 w-full max-w-6xl mx-auto font-mono">
      <div className="flex items-center justify-between mb-6">
        <h1 className="text-3xl font-bold">Transactions</h1>
        {!walletIdsToLoad && (
          <WalletLister onSelect={(wallets) => {
            setWalletIds(wallets.map((w) => w.id!));
            setRefetchTrigger(prev => prev + 1);
          }}
          />
        )}
        <CategoryLister onSelect={(categories) => {
          setCategoryIds(categories.map((c) => c.id!));
          setRefetchTrigger(prev => prev + 1);
        }}
        />
        <div className="flex flex-col">
          <Label htmlFor="date" className="mb-2">Date Range:</Label>
          <Popover>
            <PopoverTrigger asChild>
              <Button
                variant="outline"
                id="date-picker-range"
                className="justify-start px-2.5 font-normal"
              >
                <CalendarIcon />
                {date?.from ? (
                  date.to ? (
                    <>
                      {format(date.from, "LLL dd, y")} -{" "}
                      {format(date.to, "LLL dd, y")}
                    </>
                  ) : (
                    format(date.from, "LLL dd, y")
                  )
                ) : (
                  <span>Pick a date</span>
                )}
              </Button>
            </PopoverTrigger>
            <PopoverContent className="w-auto p-0" align="start">
              <Calendar
                mode="range"
                defaultMonth={date?.from}
                selected={date}
                onSelect={(dateRange) => {
                  if (dateRange?.from) dateRange.from = normalizeDate(dateRange.from);
                  if (dateRange?.to) dateRange.to = normalizeDate(dateRange.to);
                  setDate(dateRange);
                  setRefetchTrigger(prev => prev + 1);
                }}
                numberOfMonths={1}
              />
            </PopoverContent>
          </Popover>
        </div>
        <TransactionDialog onTransactionAdded={() => setRefetchTrigger(prev => prev + 1)}>
          <Button><Plus />Add Transaction</Button>
        </TransactionDialog>
      </div>
      <div className="w-full max-w-6xl mx-auto font-mono">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead colSpan={1}>Transaction</TableHead>
              <TableHead colSpan={1}>Wallet</TableHead>
              <TableHead colSpan={1}>Date</TableHead>
              <TableHead colSpan={1}>Category</TableHead>
              <TableHead colSpan={1}>Amount</TableHead>
              <TableHead colSpan={1}>Created By</TableHead>
              <TableHead className="text-right" colSpan={1}>
                Actions
              </TableHead>
            </TableRow>
          </TableHeader>
          <TableBody>
            {isLoading ? (
              <TableRow>
                <TableCell colSpan={numOfColumns} className="text-center">
                  Loading...
                </TableCell>
              </TableRow>
            ) : transactions.length === 0 ? (
              <TableRow>
                <TableCell colSpan={numOfColumns} className="text-center">
                  No transactions found.
                </TableCell>
              </TableRow>
            ) : (
              transactions.map((transaction) => (
                <TableRow key={transaction.id} style={{ backgroundColor: transaction.amount && transaction.amount < 0 ? 'rgba(255, 0, 0, 0.1)' : 'rgba(0, 255, 0, 0.1)' }}>
                  <TableCell>
                    <TransactionItem name={transaction.name} isNegative={transaction.amount < 0} />
                  </TableCell>
                  <TableCell>
                    <WalletItem walletName={transaction.wallet?.name ?? "N/A"} />
                  </TableCell>
                  <TableCell>{transaction.date ? formatDate(new Date(transaction.date)) : "N/A"}</TableCell>
                  <TableCell>
                    {transaction.category ? <CategoryItem color={transaction.category.color} icon={transaction.category.icon} name={transaction.category.name!} /> : ""}
                  </TableCell>
                  <TableCell>{formatAmount(transaction.amount)}</TableCell>
                  <TableCell>
                    <UserAvatarItem userName={transaction.createdByUser?.userName ?? "N/A"} userId={transaction.createdByUser?.id!} />
                  </TableCell>
                  <TableCell className="text-right">
                    <TransactionDialog transaction={transaction} onTransactionAdded={() => setRefetchTrigger(prev => prev + 1)}>
                      <Button variant="link">
                        Edit
                      </Button>
                    </TransactionDialog>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
          <TableFooter>
            <TableRow>
              <TableCell colSpan={numOfColumns - 1}>Total</TableCell>
              <TableCell className="text-right">{`$${transactions.reduce((sum, transaction) => sum + (transaction.amount || 0), 0).toFixed(2)}`}</TableCell>
            </TableRow>
          </TableFooter>
        </Table>
      </div>
    </div>
  )
}

export default TransactionsPage