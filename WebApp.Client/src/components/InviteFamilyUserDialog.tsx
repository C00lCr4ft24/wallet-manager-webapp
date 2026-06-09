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
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { Plus } from "lucide-react"
import { useState } from "react"
import familyService from "@/services/familyService"
import { CreateFamilyInviteDto, ProblemDetails } from "@/generated/dtos"
import { toast } from "sonner"

function InviteFamilyUserDialog({onInviteSent}: { onInviteSent?: () => void }) {
  const [email, setEmail] = useState<string>('')
  const [isOpen, setIsOpen] = useState<boolean>(false)

  const handleSubmit = async () => {
    try {
      await familyService.SendInvite(new CreateFamilyInviteDto({ userEmail: email }))
      toast.success("Invite sent successfully!", { position: 'bottom-center' })
      onInviteSent && onInviteSent()
      setIsOpen(false)
    } catch (error) {
      if(error instanceof ProblemDetails) {
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
              <Label htmlFor="email-1">Email</Label>
              <Input id="email-1" name="email" type="email" value={email} onChange={(e) => setEmail(e.target.value)} onKeyDown={(e) => { if (e.key === 'Enter') { handleSubmit() } }} />
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

export default InviteFamilyUserDialog