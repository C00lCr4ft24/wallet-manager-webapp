import AddWalletDialog from "@/components/AddWalletDialog"
import InviteWalletUserDialog from "@/components/InviteWalletUserDialog"
import { Button } from "@/components/ui/button"
import WalletInviteList from "@/components/WalletInviteList"
import { WalletList } from "@/components/WalletList"
import type { WalletHeaderDto } from "@/generated/dtos"
import { Plus } from "lucide-react"
import { useState } from "react"

function WalletsPage() {
  const [refetchTrigger, setRefetchTrigger] = useState(0)
  const [wallets, setWallets] = useState<WalletHeaderDto[]>([])
  return (
    <div className="px-12 py-12 flex h-screen">
      <div className="w-1/2 pr-12">
        <div className="flex items-center justify-between mb-6">
          <h1 className="text-3xl font-bold">Wallets</h1>
          <AddWalletDialog onWalletAdded={() => setRefetchTrigger(prev => prev + 1)}>
            <Button><Plus />Add Wallet</Button>
          </AddWalletDialog>
        </div>
        <WalletList refetchTrigger={refetchTrigger} onWalletsLoaded={setWallets} />
      </div>
      <div className="w-1/2 pl-12 border-l">
        <div className="flex items-center justify-between mb-6">
          <h1 className="text-3xl font-bold">Sent Invites</h1>
          <InviteWalletUserDialog wallets={wallets} onInviteSent={() => setRefetchTrigger(prev => prev + 1)} />
        </div>
        <WalletInviteList loadType="sent" walletId={null} refetchTrigger={refetchTrigger} />
        <div className="flex items-center justify-between mb-6 mt-12">
          <h1 className="text-3xl font-bold">Received Invites</h1>
        </div>
        <WalletInviteList loadType="received" walletId={null} refetchTrigger={refetchTrigger} onInviteResponded={() => setRefetchTrigger(prev => prev + 1)} />
      </div>
    </div>
  )
}

export default WalletsPage