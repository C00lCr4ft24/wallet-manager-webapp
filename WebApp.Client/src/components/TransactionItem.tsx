import { ArrowDown, ArrowUp } from "lucide-react";
import { Item, ItemContent, ItemMedia, ItemTitle } from "./ui/item"


function TransactionItem({ name, isNegative }: { name: string; isNegative: boolean }) {
  return (
    <Item variant="default" className="w-full">
      <ItemMedia>
        {isNegative ? (<ArrowDown />) : (<ArrowUp />)}
      </ItemMedia>
      <ItemContent>
        <ItemTitle>{name}</ItemTitle>
      </ItemContent>
    </Item>
  )
}

export default TransactionItem