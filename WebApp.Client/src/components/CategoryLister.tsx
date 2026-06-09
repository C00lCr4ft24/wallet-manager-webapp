
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
import { CategoryHeaderDto, ProblemDetails } from "@/generated/dtos"
import { useEffect, useState } from "react"
import { toast } from "sonner"
import { Label } from "./ui/label"
import categoryService from "@/services/categoryService"

function CategoryLister({ onSelect }: { onSelect?: (categories: CategoryHeaderDto[]) => void }) {
  const [categories, setCategories] = useState<CategoryHeaderDto[]>([])
  const [selectedCategories, setSelectedCategories] = useState<CategoryHeaderDto[]>([])
  const anchor = useComboboxAnchor()

  useEffect(() => {
    const fetchCategories = async () => {
      try {
        const data = await categoryService.GetCategories()
        setCategories(data)
      } catch (e) {
        if (e instanceof ProblemDetails) {
          toast.error(e.detail, { description: e.reason, position: 'bottom-center' })
        }
      }
    }
    fetchCategories()
  }, [])

  return (
    <div className="flex flex-col">
      <Label className="w-full mb-2">Filter by Categories:</Label>
      <Combobox
        multiple
        autoHighlight
        items={categories}
        onValueChange={(values: CategoryHeaderDto[]) => { setSelectedCategories(values); onSelect && onSelect(values); }}
        value={selectedCategories}
      >
        <ComboboxChips ref={anchor} className="max-w-xs">
          <ComboboxValue >
            {(values) => (
              <>
                {values.map((value: CategoryHeaderDto) => (
                  <ComboboxChip key={value.id} style={{ backgroundColor: value.color ? value.color + "20" : undefined }}>
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
                style={{ backgroundColor: item.color ? item.color + "20" : undefined }}
              >
                {item.icon} {item.name}
              </ComboboxItem>
            )}
          </ComboboxList>
        </ComboboxContent>
      </Combobox>
    </div>
  )
}

export default CategoryLister