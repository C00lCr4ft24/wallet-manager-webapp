import { Button } from "@/components/ui/button"

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
import { ProblemDetails, RespondToInviteDto, WalletInviteDto } from "@/generated/dtos"
import AreYouSureDialog from "./AreYouSureDialog"
import { Trash2, MailQuestionMark } from "lucide-react"
import walletService from "@/services/walletService"
import UserAvatarItem from "./UserAvatarItem"
import { formatDate, getBgColorFromId } from "@/lib/utils"
import WalletItem from "./WalletItem"

function WalletInviteList({ loadType, walletId, refetchTrigger, onInviteResponded }: { loadType: "sent" | "received"; walletId: number | null; refetchTrigger?: number; onInviteResponded?: () => void }) {
  const [invites, setInvites] = useState<WalletInviteDto[]>([])
  const [isLoading, setIsLoading] = useState<boolean>(true)

  const handleRespond = async (id: number, accept: boolean) => {
    try {
      await walletService.RespondToInvite(new RespondToInviteDto({ id: id, accept: accept }))
      toast.success(accept ? "Invite accepted!" : "Invite declined!")
      setInvites(invites.filter(invite => invite.id !== id))
      onInviteResponded && onInviteResponded()
    } catch (e) {
      if (e instanceof ProblemDetails) {
        toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
      }
    }
  }

  useEffect(() => {
    const fetchInvites = async () => {
      const tempInvites: WalletInviteDto[] = []
      try {
        let data: WalletInviteDto[] = []
        if (loadType === "sent" && walletId) {
          data = await walletService.GetSentInvites(walletId)
        }
        if (loadType === "sent" && !walletId) {
          data = await walletService.GetAllSentInvites()
        }
        if (loadType === "received" && walletId) {
          data = await walletService.GetReceivedInvites(walletId)
        }
        if (loadType === "received" && !walletId) {
          data = await walletService.GetAllReceivedInvites()
        }
        tempInvites.push(...data)
        setInvites(tempInvites)
      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      } finally {
        setIsLoading(false)
      }
    }
    fetchInvites()
  }, [refetchTrigger])

  const numOfColumns = 6

  return (
    <div className="w-full max-w-6xl mx-auto font-mono">
      <Table>
        <TableHeader>
          <TableRow>
            <TableHead colSpan={1}>Wallet's Name</TableHead>
            <TableHead colSpan={1}>Sent By</TableHead>
            <TableHead colSpan={1}>Sent To</TableHead>
            <TableHead colSpan={1}>Sent At</TableHead>
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
          ) : invites.length === 0 ? (
            <TableRow>
              <TableCell colSpan={numOfColumns} className="text-center">
                No invites found.
              </TableCell>
            </TableRow>
          ) : (
            invites.map((invite) => (
              <TableRow key={invite.id} style={{ backgroundColor: getBgColorFromId(invite.wallet?.id!) + "20" }}>
                <TableCell>
                  <WalletItem walletName={invite.wallet?.name ?? "N/A"} />
                </TableCell>
                <TableCell>
                  <UserAvatarItem userId={invite.inviterUser?.id ?? 0} userName={invite.inviterUser?.userName ?? 'N/A'} />
                </TableCell>
                <TableCell>
                  <UserAvatarItem userId={invite.invitedUser?.id ?? 0} userName={invite.invitedUser?.userName ?? 'N/A'} />
                </TableCell>
                <TableCell>{invite.createdAt ? formatDate(new Date(invite.createdAt)) : "N/A"}</TableCell>
                <TableCell>{invite.isAccepted ? "Accepted" : "Pending"}</TableCell>
                <TableCell className="text-right">
                  <AreYouSureDialog
                    title={loadType === "sent" ? "Delete Invite" : "Accept Invite"}
                    description={loadType === "sent" ? "Are you sure you want to delete this invite?" : "Are you sure you want to accept this invite?"}
                    confirmTitle={loadType === "sent" ? "Delete" : "Accept"}
                    cancelTitle={loadType === "sent" ? "Cancel" : "Decline"}
                    onConfirm={() => {
                      if (loadType === "sent") {

                      }
                      if (loadType === "received" && invite.id) {
                        handleRespond(invite.id, true)
                      }
                    }}
                    onCancel={() => {
                      if (loadType === "received" && invite.id) {
                        handleRespond(invite.id, false)
                      }
                    }}
                  >
                    {loadType === "sent" ? (
                      <Button variant="ghost" size="sm">
                        <Trash2 color="red" />
                      </Button>
                    ) : (
                      <Button variant="ghost" size="sm">
                        <MailQuestionMark />
                      </Button>
                    )}
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

export default WalletInviteList