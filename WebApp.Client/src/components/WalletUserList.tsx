import { MoreHorizontalIcon } from "lucide-react"

import { Button } from "@/components/ui/button"
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "@/components/ui/dropdown-menu"

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
import { ProblemDetails, WalletUserDto, } from "@/generated/dtos"
import { formatDate } from "@/lib/utils"
import UserAvatarItem from "./UserAvatarItem"
import { useNavigate } from "react-router-dom"
import walletService from "@/services/walletService"

export function WalletUserList({ walletId }: { walletId: number }) {
  const [wUsers, setUsers] = useState<WalletUserDto[]>([])
  const [isLoading, setIsLoading] = useState<boolean>(true)

  const navigate = useNavigate()

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const data = await walletService.GetWallet(walletId, true, false)
        setUsers(data.users!)
      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      } finally {
        setIsLoading(false)
      }
    }
    fetchUsers()
  }, [])

  const numOfColumns = 4

  return (
    <div className="px-12 py-12 w-full max-w-6xl mx-auto font-mono">
      <h1 className="text-3xl font-bold">Users</h1>
      <div className="flex items-center justify-between mb-6">
        <Table>
          <TableHeader>
            <TableRow>
              <TableHead colSpan={1}>User</TableHead>
              <TableHead colSpan={1}>Role</TableHead>
              <TableHead colSpan={1}>Joined At</TableHead>
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
            ) : wUsers.length === 0 ? (
              <TableRow>
                <TableCell colSpan={numOfColumns} className="text-center">
                  No users found.
                </TableCell>
              </TableRow>
            ) : (
              wUsers.map((wu) => (
                <TableRow key={wu.id}>
                  <TableCell>
                    <UserAvatarItem userId={wu.user?.id ?? 0} userName={wu.user?.userName ?? 'N/A'} />
                  </TableCell>
                  <TableCell>{wu.isOwner ? "Owner" : "Member"}</TableCell>
                  <TableCell>{wu.joinedAt ? formatDate(new Date(wu.joinedAt)) : "N/A"}</TableCell>
                  <TableCell className="text-right">
                    <DropdownMenu>
                      <DropdownMenuTrigger asChild>
                        <Button variant="ghost" className="h-8 w-8 p-0">
                          <span className="sr-only">Open menu</span>
                          <MoreHorizontalIcon className="h-4 w-4" />
                        </Button>
                      </DropdownMenuTrigger>
                      <DropdownMenuContent align="end">
                        <DropdownMenuItem onSelect={() => navigate(`/wallets/user/${wu.id}`)}>
                          Details
                        </DropdownMenuItem>
                      </DropdownMenuContent>
                    </DropdownMenu>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </div>
    </div>
  )
}

export default WalletUserList