
import {
  Combobox,
  ComboboxChip,
  ComboboxChips,
  ComboboxChipsInput,
  ComboboxContent,
  ComboboxEmpty,
  ComboboxItem,
  ComboboxList,
  ComboboxValue,
  useComboboxAnchor,
} from "@/components/ui/combobox"
import { ProblemDetails, type WalletDataDto } from "@/generated/dtos"
import { getBgColorFromId } from "@/lib/utils"
import walletService from "@/services/walletService"
import { useEffect, useState } from "react"
import { toast } from "sonner"
import { Label } from "./ui/label"
import { Wallet } from "lucide-react"

function WalletLister({ onSelect }: { onSelect?: (wallets: WalletDataDto[]) => void }) {
  const [wallets, setWallets] = useState<WalletDataDto[]>([])
  const [selectedWallets, setSelectedWallets] = useState<WalletDataDto[]>([])
  const anchor = useComboboxAnchor()

  useEffect(() => {
    const fetchWallets = async () => {
      try {
        const data = await walletService.GetWallets(false, false)
        setWallets(data)
      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      }
    }
    fetchWallets()
  }, [])

  return (
    <div className="flex flex-col">
      <Label className="w-full mb-2">Filter by Wallets:</Label>
      <Combobox
        multiple
        autoHighlight
        items={wallets}
        onValueChange={(values: WalletDataDto[]) => { setSelectedWallets(values); onSelect && onSelect(values); }}
        value={selectedWallets}
      >
        <ComboboxChips ref={anchor} className="max-w-xs">
          <ComboboxValue >
            {(values) => (
              <>
                {values.map((value: WalletDataDto) => (
                  <ComboboxChip key={value.id} style={{ backgroundColor: value.id ? getBgColorFromId(value.id) + "20" : undefined }}>
                    {value.name}
                  </ComboboxChip>
                ))}
                <ComboboxChipsInput />
              </>
            )}
          </ComboboxValue>
        </ComboboxChips>
        <ComboboxContent anchor={anchor}>
          <ComboboxEmpty>No items found.</ComboboxEmpty>
          <ComboboxList>
            {(item) => (
              <ComboboxItem
                key={item.id}
                value={item}
                style={{ backgroundColor: item.id ? getBgColorFromId(item.id) + "20" : undefined }}
              >
                <Wallet />{item.name}
              </ComboboxItem>
            )}
          </ComboboxList>
        </ComboboxContent>
      </Combobox>
    </div>
  )
}

export default WalletLister