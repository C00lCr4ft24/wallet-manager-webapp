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
import { ProblemDetails, type FamilyUserDto } from "@/generated/dtos"
import familyService from "@/services/familyService"
import { formatDate } from "@/lib/utils"
import UserAvatarItem from "./UserAvatarItem"
import { useNavigate } from "react-router-dom"

export function FamilyUserList({ refetchTrigger }: { refetchTrigger: number }) {
  const [users, setUsers] = useState<FamilyUserDto[]>([])
  const [isLoading, setIsLoading] = useState<boolean>(true)

  const navigate = useNavigate()

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        const data = await familyService.GetFamilyUsers()
        setUsers(data)
      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      } finally {
        setIsLoading(false)
      }
    }
    fetchUsers()
  }, [refetchTrigger])

  const numOfColumns = 4

  return (
    <div className="w-full max-w-6xl mx-auto font-mono">
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
          ) : users.length === 0 ? (
            <TableRow>
              <TableCell colSpan={numOfColumns} className="text-center">
                No users found.
              </TableCell>
            </TableRow>
          ) : (
            users.map((user) => (
              <TableRow key={user.id}>
                <TableCell>
                  <UserAvatarItem userId={user.user.id ?? 0} userName={user.user.userName ?? 'N/A'} />
                </TableCell>
                <TableCell>{user.isOwner ? "Owner" : "Member"}</TableCell>
                <TableCell>{user.joinedAt ? formatDate(new Date(user.joinedAt)) : "N/A"}</TableCell>
                <TableCell className="text-right">
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" className="h-8 w-8 p-0">
                        <span className="sr-only">Open menu</span>
                        <MoreHorizontalIcon className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <DropdownMenuItem onSelect={() => navigate(`/family/user/${user.id}`)}>
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
  )
}

export default FamilyUserList