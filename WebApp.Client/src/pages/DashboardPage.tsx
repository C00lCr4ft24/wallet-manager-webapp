import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card"
import { Label } from "@/components/ui/label"
import { GetAllTransactionsDto, ProblemDetails, type TransactionDataDto } from "@/generated/dtos"
import transactionService from "@/services/transactionService"
import { addDays, format, subDays } from "date-fns"
import { useEffect, useState } from "react"
import { toast } from "sonner"
import { Table, TableBody, TableCell, TableFooter, TableHead, TableHeader, TableRow } from "@/components/ui/table"
import TransactionDialog from "@/components/TransactionDialog"
import { Button } from "@/components/ui/button"
import { Bar } from "react-chartjs-2"
import { Chart as ChartJS, CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend, ArcElement } from "chart.js"
import { Pie } from 'react-chartjs-2'
import CategoryItem from "@/components/CategoryItem"
import WalletItem from "@/components/WalletItem"
import TransactionItem from "@/components/TransactionItem"
import { CalendarIcon, Plus } from "lucide-react"
import { Popover, PopoverContent, PopoverTrigger } from "@/components/ui/popover"
import CategoryLister from "@/components/CategoryLister"
import type { DateRange } from "react-day-picker"
import { Calendar } from "@/components/ui/calendar"
import { formatDate } from "@/lib/utils"
import UserAvatarItem from "@/components/UserAvatarItem"

ChartJS.register(CategoryScale, LinearScale, BarElement, Title, Tooltip, Legend, ArcElement)

function DashboardPage() {
  const [refetchTrigger, setRefetchTrigger] = useState<number>(0)
  const [isLoading, setIsLoading] = useState<boolean>(true)

  const numOfColumns = 7

  const [categoryIds, setCategoryIds] = useState<number[]>([])
  const [transactions, setTransactions] = useState<TransactionDataDto[]>([])

  const [totalIncome, setTotalIncome] = useState<number>(0)
  const [totalExpense, setTotalExpense] = useState<number>(0)
  const [overall, setOverall] = useState<number>(0)

  const [categoryLabels, setCategoryLabels] = useState<string[]>([])
  const [categoryValues, setCategoryValues] = useState<number[]>([])
  const [categoryColors, setCategoryColors] = useState<string[]>([])

  const normalizeDate = (d: Date) => {
    const normalized = new Date(d)
    normalized.setHours(0, 0, 0, 0)
    return normalized
  }

  const options = {
    responsive: true,
    plugins: {
      title: {
        display: true,
        text: 'Income vs Expense',
      },
    },
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
          startDate: subDays(date?.from!, 1),
          endDate: addDays(date?.to!, 1)
        }))
        data = data.sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime())
        setTransactions(data)
        setTotalIncome(data.reduce((sum, transaction) => sum + (transaction.amount && transaction.amount > 0 ? transaction.amount : 0), 0))
        setTotalExpense(data.reduce((sum, transaction) => sum + (transaction.amount && transaction.amount < 0 ? transaction.amount : 0), 0))
        setOverall(data.reduce((sum, transaction) => sum + (transaction.amount || 0), 0))

        const categoryMap = new Map<string, number>()
        const categoryColorMap = new Map<string, string>()

        console.log("All transactions:", data)

        data.forEach(transaction => {
          if (transaction.amount < 0) {
            const categoryName = transaction.category ? transaction.category.name! : 'No Category'
            const currentAmount = categoryMap.get(categoryName) || 0
            categoryMap.set(categoryName, currentAmount + Math.abs(transaction.amount))
            if (!categoryColorMap.has(categoryName)) {
              categoryColorMap.set(categoryName, transaction.category ? transaction.category.color! : '#aaa')
            }
          }
        })

        const labels = Array.from(categoryMap.keys())
        const values = Array.from(categoryMap.values())
        const colors = labels.map(label => categoryColorMap.get(label) || '#aaa')

        setCategoryLabels(labels)
        setCategoryValues(values)
        setCategoryColors(colors)

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
    <>
      <Label className="text-4xl font-bold py-2 ml-4">Dashboard</Label>
      <div className="grid grid-cols-3 gap-4 h-full mx-auto py-10 px-4">
        <div className="grid grid-rows-3 gap-4">
          <Card>
            <CardHeader>
              <CardTitle className="text-4xl font-semibold underline">Income:</CardTitle>
            </CardHeader>
            <CardContent className="flex text-4xl items-right justify-end">
              <span className="font-bold">
                {formatAmount(totalIncome)}
              </span>
            </CardContent>
          </Card>
          <Card>
            <CardHeader>
              <CardTitle className="text-4xl font-semibold underline">Expense:</CardTitle>
            </CardHeader>
            <CardContent className="flex text-4xl items-right justify-end">
              <span className="font-bold">
                {formatAmount(totalExpense)}
              </span>
            </CardContent>
          </Card>
          <Card>
            <CardHeader>
              <CardTitle className="text-4xl font-semibold underline">Overall:</CardTitle>
            </CardHeader>
            <CardContent className="flex text-4xl items-right justify-end">
              <span className="font-bold">
                {formatAmount(overall)}
              </span>
            </CardContent>
          </Card>
        </div>
        <Card>
          <CardHeader>
            <CardTitle className="justify-center flex">Income vs Expense</CardTitle>
          </CardHeader>
          <CardContent>
            <Bar options={options} data={{
              labels: ['Income', 'Expense'],
              datasets: [
                {
                  label: 'Amount',
                  data: [totalIncome, Math.abs(totalExpense)],
                  backgroundColor: ['#22c55e', '#ef4444']
                }
              ]
            }} />
          </CardContent>
        </Card>
        <div className="w-full max-w-4xl mx-auto">
          <Card>
            <CardHeader>
              <CardTitle className="justify-center flex">Expenses by Category</CardTitle>
            </CardHeader>
            <CardContent>
              <div className="w-full h-96">
                <Pie
                  data={{
                    labels: categoryLabels,
                    datasets: [
                      {
                        data: categoryValues,
                        backgroundColor: categoryColors.slice(0, categoryLabels.length),
                        borderColor: '#444',
                        borderWidth: 1
                      }
                    ]
                  }}
                  options={{
                    responsive: true,
                    maintainAspectRatio: false,
                    plugins: {
                      legend: {
                        position: 'right' as const,
                      },
                      tooltip: {
                        callbacks: {
                          label: function (context: any) {
                            const total = categoryValues.reduce((a, b) => a + b, 0)
                            const percentage = ((context.parsed / total) * 100).toFixed(1)
                            return `$${context.parsed.toFixed(2)} (${percentage}%)`
                          }
                        }
                      }
                    }
                  }}
                />
              </div>
            </CardContent>
          </Card>
        </div>
      </div>
      <div className="px-12 py-12 w-full max-w-6xl mx-auto font-mono">
        <div className="flex items-center justify-between mb-6">
          <h1 className="text-3xl font-bold">Transactions</h1>
          <CategoryLister onSelect={(categories) => {
            setCategoryIds(categories.map((c) => c.id!))
            setRefetchTrigger(prev => prev + 1)
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
                    if (dateRange?.from) dateRange.from = normalizeDate(dateRange.from)
                    if (dateRange?.to) dateRange.to = normalizeDate(dateRange.to)
                    setDate(dateRange)
                    setRefetchTrigger(prev => prev + 1)
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
    </>
  )
}

export default DashboardPage