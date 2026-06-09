import { CreateWalletDto, ProblemDetails } from "@/generated/dtos"
import walletService from "@/services/walletService"
import { useState } from "react"
import { toast } from "sonner"
import { Dialog, DialogClose, DialogContent, DialogDescription, DialogFooter, DialogHeader, DialogTitle, DialogTrigger } from "./ui/dialog"
import { Button } from "./ui/button"
import { Field, FieldGroup } from "./ui/field"
import { Input } from "./ui/input"
import { Label } from "./ui/label"

function AddWalletDialog({ children, onWalletAdded }: { children: React.ReactNode; onWalletAdded?: () => void }) {
  const [open, setOpen] = useState(false)

  const [name, setName] = useState<string>('')
  const [amount, setAmount] = useState<string>('')

  const handleSubmit = async () => {
    if (!name || !amount) {
      toast.error("Please fill in all fields!", { position: 'bottom-center' })
      return
    }
    try {
      await walletService.CreateWallet(
        new CreateWalletDto({
          name: name,
          balance: parseFloat(amount)
        }))
      toast.success("Wallet created successfully!", { position: 'bottom-center' })
      setOpen(false)
      onWalletAdded && onWalletAdded()
    } catch (e) {
      if (e instanceof ProblemDetails) {
        toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
      }
    }
  }
  return (
    <Dialog open={open} onOpenChange={(isOpen) => setOpen(isOpen)}>
      <form>
        <DialogTrigger asChild>
          {children}
        </DialogTrigger>
        <DialogContent className="sm:max-w-sm">
          <DialogHeader>
            <DialogTitle>Add Wallet</DialogTitle>
            <DialogDescription>
              Enter the details of the wallet you want to create.
            </DialogDescription>
          </DialogHeader>
          <FieldGroup>
            <Field>
              <Label htmlFor="name">Name</Label>
              <Input id="name" name="name" type="text" value={name} onChange={(e) => setName(e.target.value)} onKeyDown={(e) => { if (e.key === 'Enter') { handleSubmit() } }} />
            </Field>
            <Field>
              <Label htmlFor="amount">Amount</Label>
              <Input id="amount" name="amount" type="number" value={amount} onChange={(e) => setAmount(e.target.value)} onKeyDown={(e) => { if (e.key === 'Enter') { handleSubmit() } }} />
            </Field>
          </FieldGroup>
          <DialogFooter>
            <DialogClose asChild>
              <Button variant="outline">Cancel</Button>
            </DialogClose>
            <Button type="submit" onClick={() => { handleSubmit() }}>
              Create Wallet
            </Button>
          </DialogFooter>
        </DialogContent>
      </form>
    </Dialog>
  )
}

export default AddWalletDialog