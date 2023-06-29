import TextField from "@mui/material/TextField";
import FilterType from "../Objects/FilterType";
import PriceRanges from "../Objects/PriceRange";

export default function PriceRangeInput(selectedFilterTypes : FilterType, priceRange: PriceRanges){
    const handleLowRange = (e: React.ChangeEvent<HTMLInputElement>) => {
        priceRange.low = e.currentTarget.value;
      };
      const handleHighRange = (e: React.ChangeEvent<HTMLInputElement>) => {
        priceRange.high = e.currentTarget.value;
      };


}