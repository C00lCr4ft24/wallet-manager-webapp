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
import { ProblemDetails, RespondToInviteDto, type FamilyInviteDto } from "@/generated/dtos"
import familyService from "@/services/familyService"
import AreYouSureDialog from "./AreYouSureDialog"
import { Trash2, MailQuestionMark } from "lucide-react"
import UserAvatarItem from "./UserAvatarItem"
import { formatDate } from "@/lib/utils"

function FamilyInviteList({ loadType, refetchTrigger, onAction }: { loadType: "sent" | "received", refetchTrigger?: number, onAction?: () => void }) {
  const [invites, setInvites] = useState<FamilyInviteDto[]>([])
  const [isLoading, setIsLoading] = useState<boolean>(true)

  const numOfColumns = 5

  const handleRespond = async (id: number, accept: boolean) => {
    try {
      await familyService.RespondToInvite(new RespondToInviteDto({ id: id, accept: accept }))
      toast.success(accept ? "Invite accepted!" : "Invite declined!")
      setInvites(invites.filter(invite => invite.id !== id))
      onAction && onAction()
    } catch (e) {
      if (e instanceof ProblemDetails) {
        toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
      }
    }
  }

  const handleDelete = async (id: number) => {
    try {
      await familyService.DeleteInvite(id)
      toast.success("Invite deleted!")
      setInvites(invites.filter(invite => invite.id !== id))
      onAction && onAction()
    } catch (e) {
      if (e instanceof ProblemDetails) {
        toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
      }
    }
  }

  useEffect(() => {
    const fetchInvites = async () => {
      const tempInvites: FamilyInviteDto[] = []
      try {
        if (loadType === "sent") {
          const data = await familyService.GetSentInvites()
          tempInvites.push(...data)
        }
        if (loadType === "received") {
          const data = await familyService.GetReceivedInvites()
          tempInvites.push(...data)
        }
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

  return (
    <div className="w-full max-w-6xl mx-auto font-mono">
      <Table>
        <TableHeader>
          <TableRow>
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
              <TableRow key={invite.id}>
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
                        handleDelete(invite.id!)
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

export default FamilyInviteList