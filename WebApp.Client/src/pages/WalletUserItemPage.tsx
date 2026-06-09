import { ProblemDetails, WalletUserDto } from "@/generated/dtos";
import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { toast } from "sonner";
import { Button } from "@/components/ui/button"
import {
  Card,
  CardAction,
  CardContent,
  CardDescription,
  CardFooter,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { formatDate } from "@/lib/utils";
import UserAvatarItem from "@/components/UserAvatarItem";
import walletService from "@/services/walletService";


function WalletUserItemPage() {
  const { walletUserId } = useParams()
  const [user, setUser] = useState<WalletUserDto | undefined>(undefined)
  const [invitedByUser, setInvitedByUser] = useState<WalletUserDto | undefined>(undefined)

  const toggleOwnerStatus = async () => {
    try {
      const updatedUser = await walletService.UpdateWalletUser(user?.wallet?.id!, user?.id!, !user?.isOwner)
      setUser(updatedUser)
      toast.success(`User is now ${updatedUser.isOwner ? "an Owner" : "a Member"}`, { position: 'bottom-center' })
    } catch (e) {
      if (e instanceof ProblemDetails) {
        toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
      }
    }
  }

  useEffect(() => {
    const fetchUser = async () => {
      try {
        const userData: WalletUserDto = await walletService.GetWalletUser(parseInt(walletUserId!))
        setUser(userData)
        const invitedByUserData = userData.updatedByUserId ? await walletService.GetWalletUser(userData.updatedByUserId) : undefined
        setInvitedByUser(invitedByUserData)
      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      }
    }
    fetchUser()
  }, [walletUserId])

  return (
    <div className="w-full max-w-6xl mx-auto flex h-screen items-center justify-center">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>Wallet User Details</CardTitle>
          <CardDescription>
            View and manage your wallet user details
          </CardDescription>
          <CardAction>
            <Button variant="link" onClick={() => window.history.back()}>
              Back
            </Button>
          </CardAction>
        </CardHeader>
        <CardContent>
          <div className="grid w-full items-center gap-4">
            <div className="flex flex-col space-y-1.5">
              <Label htmlFor="name">User</Label>
              <UserAvatarItem userName={user?.user?.userName ?? ''} userId={user?.user?.id!} />
            </div>
            <div className="flex flex-col space-y-1.5">
              <Label htmlFor="family">Wallet</Label>
              <Input id="family" value={user?.wallet?.name || ''} disabled />
            </div>
            <div className="flex flex-col space-y-1.5">
              <Label htmlFor="role">Role</Label>
              <Input id="role" value={user?.isOwner ? "Owner" : "Member"} disabled />
            </div>
            <div className="flex flex-col space-y-1.5">
              <Label htmlFor="joinedAt">Joined At</Label>
              <Input
                id="joinedAt"
                value={user?.joinedAt ? formatDate(new Date(user.joinedAt)) : ''}
                disabled
              />
            </div>
            <div className="flex flex-col space-y-1.5">
              <Label htmlFor="invitedBy">Invited By</Label>
              <UserAvatarItem userName={invitedByUser?.user?.userName ?? ''} userId={invitedByUser?.user?.id!} />
            </div>
          </div>
        </CardContent>
        <CardFooter className="flex-col gap-2">
          <Button
            type="submit"
            className="w-full"
            onClick={() => toggleOwnerStatus()}
          >
            Make {user?.isOwner ? "Member" : "Owner"}
          </Button>
        </CardFooter>
      </Card>
    </div>
  )
}

export default WalletUserItemPage
