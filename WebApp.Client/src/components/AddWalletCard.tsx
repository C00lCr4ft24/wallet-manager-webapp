import { Button } from "@/components/ui/button"
import { Card, CardContent, CardFooter, CardHeader, CardTitle, } from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"
import { CreateWalletDto, ProblemDetails } from "@/generated/dtos"
import walletService from "@/services/walletService"
import { useState } from "react"
import { toast } from "sonner"

export default function WalletAddCard() {

  const [name, setName] = useState<string>('')
  const [amount, setAmount] = useState<string>('')

  const handleSubmit = async () => {
    if (!name || !amount) {
      toast.error("Please fill in all fields.", { position: 'bottom-center' })
      return
    }
    try {
      await walletService.CreateWallet(
        new CreateWalletDto({
        name: name,
        balance: parseFloat(amount)
      }))
      toast.success("Wallet created successfully!", { position: 'bottom-center' })
    } catch (e) {
      if (e instanceof ProblemDetails) {
        toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
      }
    }
  }

  return (
    <Card className="w-full max-w-sm">
      <CardHeader>
        <CardTitle>Add New Wallet</CardTitle>
      </CardHeader>
      <CardContent>
        <form onSubmit={(e) => { e.preventDefault(); handleSubmit() }}>
          <div className="flex flex-col gap-6">
            <div className="grid gap-2">
              <Label htmlFor="name">Name</Label>
              <Input
                value={name}
                onChange={(e) => setName(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSubmit()}
                id="name"
                type="text"
                placeholder="..."
                required
              />
            </div>
            <div className="grid gap-2">
              <div className="flex items-center">
                <Label htmlFor="amount">Amount ($)</Label>
              </div>
              <Input
                value={amount}
                onChange={(e) => setAmount(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSubmit()}
                id="amount"
                type="number"
                placeholder="$0.00"
                required
              />
            </div>
          </div>
        </form>
      </CardContent>
      <CardFooter className="flex-col gap-2">
        <Button
          type="submit"
          className="w-full"
          onClick={() => handleSubmit()}
        >
          Add Wallet
        </Button>
        <Button variant="outline" className="w-full">
          Go Back
        </Button>
      </CardFooter>
    </Card>
  )
}