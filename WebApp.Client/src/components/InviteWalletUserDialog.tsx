import { Button } from "@/components/ui/button"
import {
  Dialog,
  DialogClose,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog"
import { Field, FieldGroup } from "@/components/ui/field"
import { Label } from "@/components/ui/label"
import { Plus } from "lucide-react"
import { useEffect, useState } from "react"
import { CreateWalletInviteDto, FamilyUserDto, ProblemDetails, WalletHeaderDto } from "@/generated/dtos"
import { toast } from "sonner"
import walletService from "@/services/walletService"
import { Select, SelectContent, SelectGroup, SelectItem, SelectTrigger, SelectValue } from "./ui/select"
import { getBgColorFromId } from "@/lib/utils"
import familyService from "@/services/familyService"
import UserAvatarItem from "./UserAvatarItem"
import userService from "@/services/userService"

function InviteWalletUserDialog({ wallets, onInviteSent }: { wallets: WalletHeaderDto[], onInviteSent?: () => void }) {

  const [selectedWallet, setSelectedWallet] = useState<number>(-1)
  const [isOpen, setIsOpen] = useState<boolean>(false)
  const [users, setUsers] = useState<FamilyUserDto[]>([])
  const [selectedUser, setSelectedUser] = useState<number>(-1)

  useEffect(() => {
    const fetchUsers = async () => {
      try {
        let fusers = await familyService.GetFamilyUsers()
        const currentUser = await userService.currentUser()
        fusers = fusers.filter(fu => fu.user.id !== currentUser.id)
        setUsers(fusers)
      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      }
    }
    if (isOpen) {
      fetchUsers()
    }
  }, [isOpen])

  const handleSubmit = async () => {
    try {
      if (selectedWallet === -1) {
        toast.error("Please select a wallet!", { position: 'bottom-center' })
        return
      }
      if (selectedUser === -1) {
        toast.error("Please select a user!", { position: 'bottom-center' })
        return
      } 
      await walletService.SendInvite(selectedWallet, new CreateWalletInviteDto({ userId: selectedUser }))
      toast.success("Invite sent successfully!", { position: 'bottom-center' })
      setIsOpen(false)
      setSelectedWallet(-1)
      onInviteSent && onInviteSent()
    } catch (error) {
      if (error instanceof ProblemDetails) {
        toast.error(error.detail, { description: error.reason, position: 'bottom-center' })
      }
    }
  }

  return (
    <Dialog open={isOpen} onOpenChange={setIsOpen}>
      <form>
        <DialogTrigger asChild>
          <Button onClick={() => { }}>
            <Plus /> Create Invite
          </Button>
        </DialogTrigger>
        <DialogContent className="sm:max-w-sm">
          <DialogHeader>
            <DialogTitle>Invite Member</DialogTitle>
            <DialogDescription>
              Enter the email of the user you want to invite.
            </DialogDescription>
          </DialogHeader>
          <FieldGroup>
            <Field>
              <Label htmlFor="user">User</Label>
              <Select value={selectedUser === -1 ? '' : selectedUser.toString()} onValueChange={(value) => setSelectedUser(parseInt(value))}>
                <SelectTrigger id="user">
                  <SelectValue placeholder="Select a user" />
                </SelectTrigger>
                <SelectContent>
                  <SelectGroup>
                    {users.map((user) => (
                      <SelectItem key={user.id} value={user.id?.toString() ?? '0'} style={{ backgroundColor: getBgColorFromId(user.id ?? 0) + '20' }}>
                        <UserAvatarItem userId={user.user.id ?? 0} userName={user.user.userName ?? 'N/A'} />
                      </SelectItem>
                    ))}
                  </SelectGroup>
                </SelectContent>
              </Select>
            </Field>
            <Field>
              <Label htmlFor="wallet">Wallet</Label>
              <Select value={selectedWallet === -1 ? '' : selectedWallet.toString()} onValueChange={(value) => setSelectedWallet(parseInt(value))}>
                <SelectTrigger id="wallet">
                  <SelectValue placeholder="Select a wallet" />
                </SelectTrigger>
                <SelectContent>
                  <SelectGroup>
                    {wallets.map((wallet) => (
                      <SelectItem key={wallet.id} value={wallet.id?.toString() ?? '0'} style={{ backgroundColor: getBgColorFromId(wallet.id ?? 0) + '20' }}>
                        {wallet.name}
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
            <Button type="submit" onClick={() => { handleSubmit() }}>
              Send Invite
            </Button>
          </DialogFooter>
        </DialogContent>
      </form>
    </Dialog>
  )
}

export default InviteWalletUserDialog