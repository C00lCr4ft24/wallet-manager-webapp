import {
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"

import { useEffect, useState } from "react"
import { toast } from "sonner"
import { LimitDto, ProblemDetails, } from "@/generated/dtos"
import { Button } from "./ui/button"
import limitService from "@/services/limitService"
import UserAvatarItem from "./UserAvatarItem"
import { differenceInDays } from "date-fns"
import { Trash2 } from "lucide-react"
import AreYouSureDialog from "./AreYouSureDialog"

export function LimitList({ refetchTrigger, onChanges }: { refetchTrigger?: number; onChanges?: () => void }) {
  const [limits, setLimits] = useState<LimitDto[]>([])
  const [isLoading, setIsLoading] = useState<boolean>(true)

  const numOfColumns = 9

  useEffect(() => {
    const fetchLimits = async () => {
      try {
        const limits = await limitService.getAllLimits()
        setLimits(limits)
        onChanges && onChanges()
      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      } finally {
        setIsLoading(false)
      }
    }
    fetchLimits()
  }, [refetchTrigger])

  const formatAmount = (amount: number | undefined) => {
    if (amount === undefined) return "N/A"
    const sign = amount < 0 ? "-" : ""
    return `${sign}$${Math.abs(amount).toFixed(2)}`
  }

  const handleDelete = async (id: number) => {
    try {
      await limitService.deleteLimit(id)
      toast.success("Limit deleted successfully!", { position: 'bottom-center' })
      setLimits(limits.filter(limit => limit.id !== id))
    } catch (e) {
      if (e instanceof ProblemDetails) {
        toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
      }
    }
  }

  return (
    <div className="w-full max-w-6xl mx-auto font-mono">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead colSpan={1}>Applied To</TableHead>
            <TableHead colSpan={1}>Max Amount</TableHead>
            <TableHead colSpan={1}>Available</TableHead>
            <TableHead colSpan={1}>Days Remaining</TableHead>
            <TableHead colSpan={1}>Include Income</TableHead>
            <TableHead colSpan={1}>Description</TableHead>
            <TableHead colSpan={1}>Status</TableHead>
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
          ) : limits.length === 0 ? (
            <TableRow>
              <TableCell colSpan={numOfColumns} className="text-center">
                No limits found.
              </TableCell>
            </TableRow>
          ) : (
            limits.map((limit: LimitDto) => (
              <TableRow key={limit.id}>
                <TableCell>
                  <UserAvatarItem userId={limit.user?.id!} userName={limit.user?.userName ?? "N/A"} />
                </TableCell>
                <TableCell>{formatAmount(limit.maxAmount)}</TableCell>
                <TableCell>{limit.maxAmount && limit.currentAmount !== undefined ? formatAmount(limit.maxAmount - limit.currentAmount) : "N/A"}</TableCell>

                <TableCell>{limit.endDate && limit.startDate !== undefined ? differenceInDays(new Date(limit.endDate), new Date(limit.startDate)) : "N/A"}</TableCell>
                <TableCell>{limit.includeIncome ? "Yes" : "No"}</TableCell>
                <TableCell>{limit.description || limit.description !== "" ? limit.description : "-"}</TableCell>
                <TableCell>{limit.isActive ? "Active" : "Inactive"}</TableCell>
                <TableCell className="text-right">
                  <AreYouSureDialog
                    title="Delete Limit"
                    description="Are you sure you want to delete this limit?"
                    confirmTitle="Delete"
                    cancelTitle="Cancel"
                    onConfirm={() => handleDelete(limit.id!)}
                    onCancel={() => {() => { }}}
                  >
                    <Button variant="ghost" size="sm">
                      <Trash2 color="red" />
                    </Button>
                  </AreYouSureDialog>
                </TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
      </Table>
    </div>
  )
}

export default LimitList