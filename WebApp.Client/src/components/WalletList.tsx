import {
  Table,
  TableBody,
  TableCell,
  TableFooter,
  TableHead,
  TableHeader,
  TableRow,
} from "@/components/ui/table"

import { useEffect, useState } from "react"
import { toast } from "sonner"
import { ProblemDetails, WalletDataDto } from "@/generated/dtos"
import walletService from "@/services/walletService"
import userService from "@/services/userService"
import { DropdownMenu, DropdownMenuContent, DropdownMenuItem, DropdownMenuTrigger } from "./ui/dropdown-menu"
import { Button } from "./ui/button"
import { MoreHorizontalIcon } from "lucide-react"
import { formatDate, getBgColorFromId } from "@/lib/utils"
import WalletItem from "./WalletItem"
import { useNavigate } from "react-router-dom"

export function WalletList({ refetchTrigger, onWalletsLoaded }: { refetchTrigger?: number; onWalletsLoaded: (wallets: WalletDataDto[]) => void }) {
  const [wallets, setWallets] = useState<WalletDataDto[]>([])
  const [isLoading, setIsLoading] = useState<boolean>(true)
  const [currentUserId, setCurrentUserId] = useState<number>(-1)

  const navigate = useNavigate()

  useEffect(() => {
    const fetchWallets = async () => {
      try {
        const data = await walletService.GetWallets(true, false)
        const currentUser = await userService.currentUser()
        setCurrentUserId(currentUser.id)
        setWallets(data)
        onWalletsLoaded(data)
      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      } finally {
        setIsLoading(false)
      }
    }
    fetchWallets()
  }, [refetchTrigger])

  const numOfColumns = 5

  return (
    <div className="w-full max-w-6xl mx-auto font-mono">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead colSpan={1}>Wallet</TableHead>
            <TableHead colSpan={1}>Your Role</TableHead>
            <TableHead colSpan={1}>Updated At</TableHead>
            <TableHead colSpan={1}>Balance</TableHead>
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
          ) : wallets.length === 0 ? (
            <TableRow>
              <TableCell colSpan={numOfColumns} className="text-center">
                No wallets found.
              </TableCell>
            </TableRow>
          ) : (
            wallets.map((wallet) => (
              <TableRow key={wallet.id} style={{ backgroundColor: getBgColorFromId(wallet.id!) + "20" }}>
                <TableCell>
                  <WalletItem walletName={wallet.name ?? "N/A"} />
                </TableCell>
                <TableCell>{wallet.users?.find((u) => u.user?.id === currentUserId)?.isOwner === true ? "Owner" : "Member"}</TableCell>
                <TableCell>{wallet.updatedAt ? formatDate(new Date(wallet.updatedAt)) : "N/A"}</TableCell>
                <TableCell>{"$" + (wallet.balance?.toFixed(2) ?? "N/A")}</TableCell>
                <TableCell className="text-right">
                  <DropdownMenu>
                    <DropdownMenuTrigger asChild>
                      <Button variant="ghost" className="h-8 w-8 p-0">
                        <span className="sr-only">Open menu</span>
                        <MoreHorizontalIcon className="h-4 w-4" />
                      </Button>
                    </DropdownMenuTrigger>
                    <DropdownMenuContent align="end">
                      <DropdownMenuItem
                      onClick={() => navigate(`/wallets/${wallet.id}`)}
                      >
                        Details
                      </DropdownMenuItem>
                    </DropdownMenuContent>
                  </DropdownMenu>
                </TableCell>
              </TableRow>
            ))
          )}
        </TableBody>
        <TableFooter>
          <TableRow>
            <TableCell colSpan={numOfColumns - 1}>Total</TableCell>
            <TableCell className="text-right">{`$${wallets.reduce((sum, wallet) => sum + (wallet.balance || 0), 0).toFixed(2)}`}</TableCell>
          </TableRow>
        </TableFooter>
      </Table>
    </div>
  )
}

export default WalletList