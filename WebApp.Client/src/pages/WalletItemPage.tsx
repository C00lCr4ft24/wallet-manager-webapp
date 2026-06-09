import { ProblemDetails, UpdateWalletDto, type WalletDataDto } from "@/generated/dtos"
import walletService from "@/services/walletService"
import { useEffect, useState } from "react"
import { useParams } from "react-router-dom"
import { toast } from "sonner"
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
import WalletUserList from "@/components/WalletUserList"
import TransactionsPage from "./TransactionsPage"
import AreYouSureDialog from "@/components/AreYouSureDialog"
import { Field } from "@/components/ui/field"

function WalletItemPage() {
  const { walletId } = useParams()
  const [wallet, setWallet] = useState<WalletDataDto | undefined>(undefined)
  const [editMode, setEditMode] = useState<boolean>(false)
  const [name, setName] = useState<string>('')
  const [balance, setBalance] = useState<number | undefined>(undefined)

  useEffect(() => {
    const fetchWallet = async () => {
      try {
        const data = await walletService.GetWallet(parseInt(walletId!), true, true)
        setWallet(data)
        wallet && setName(wallet.name || '')
        wallet && setBalance(wallet.balance)
      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      }
    }
    fetchWallet()
  }, [walletId])

  const formatAmount = (amount: number | undefined) => {
    if (amount === undefined) return "N/A"
    return `${Math.abs(amount).toFixed(2)}`
  }

  const handleSaveChanges = async () => {
    try {
      const updatedWallet = await walletService.EditWallet(parseInt(walletId!), new UpdateWalletDto({
        name: name,
        balance: balance
      }))
      setWallet(updatedWallet)
      setEditMode(false)
      toast.success('Wallet updated successfully!', { position: 'bottom-center' })
    } catch (e) {
      if (e instanceof ProblemDetails) {
        toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
      }
    }
  }

  return (
    <div className="w-full max-w-6xl mx-auto flex flex-col items-center justify-center">
      <Card className="w-full max-w-sm">
        <CardHeader>
          <CardTitle>Wallet Details</CardTitle>
          <CardDescription>
            View and manage the wallet details
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
              <Label htmlFor="name">Wallet Name</Label>
              <Input id="name" value={name} disabled={!editMode} onChange={(e) => setName(e.target.value)} />
            </div>

            <div className="flex flex-col space-y-1.5">
              <Label htmlFor="balance">Balance</Label>
              <Input type="number" id="balance" value={formatAmount(balance) || ''} disabled />
            </div>
          </div>
        </CardContent>
        <CardFooter className="flex-col gap-2">
          <Field orientation="vertical">
            <Button
              type="submit"
              variant={editMode ? "default" : "outline"}
              className="w-full"
              onClick={() => {
                if (editMode) {
                  handleSaveChanges()
                  setEditMode(false)
                } else {
                  setEditMode(true)
                }
              }}
            >
              {editMode ? "Save Changes" : "Edit Wallet"}
            </Button>
            <AreYouSureDialog
              title="Delete Wallet"
              description="Are you sure you want to delete this wallet? This action cannot be undone."
              confirmTitle="Delete"
              onConfirm={async () => {
                try {
                  await walletService.DeleteWallet(parseInt(walletId!))
                  toast.success('Wallet deleted successfully!', { position: 'bottom-center' })
                  window.history.back()
                } catch (e) {
                  if (e instanceof ProblemDetails) {
                    toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
                  }
                }
              }}
              cancelTitle="Cancel"
              onCancel={() => { }}
            >
              <Button
                variant="destructive"
                className="w-full"
              >
                Delete Wallet
              </Button>
            </AreYouSureDialog>
          </Field>
        </CardFooter>
      </Card>
      <WalletUserList walletId={parseInt(walletId!)} />
      <TransactionsPage walletIdsToLoad={[parseInt(walletId!)]} />
    </div>
  )
}

export default WalletItemPage