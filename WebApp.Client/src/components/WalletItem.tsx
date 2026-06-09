import { Wallet } from "lucide-react";
import { Item, ItemContent, ItemMedia, ItemTitle } from "@/components/ui/item";


function WalletItem({walletName}: {walletName: string}) {
  return (
    <Item variant="default" className="w-full">
      <ItemMedia>
        <Wallet />
      </ItemMedia>
      <ItemContent>
        <ItemTitle>{walletName}</ItemTitle>
      </ItemContent>
    </Item>
  )
}

export default WalletItem