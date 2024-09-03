'use server'
import { FieldValues } from "react-hook-form";
import { fetchWrapper } from "../../lib/fetchWrapper";
import { PagedResult, Auction } from "../types";
import { revalidatePath } from "next/cache";

export async function getData(query: string): Promise<PagedResult<Auction>> {
    // const res = await fetch(`http://localhost:6001/search${query}`);

    // if (!res.ok) throw new Error('Failed to fetch data');

    // return res.json();
    return await fetchWrapper.get(`search${query}`)
}

export async function updateAuctionTest() {
    const data = {
        mileage: Math.floor(Math.random() * 10000) + 1
    }
    // const res = await fetch('http://localhost:6001/auctions', {
    //     method: 'PUT',
    //     headers: {},
    //     body: JSON.stringify(data)
    // })

    // if (!res.ok) return { status: res.status, message: res.statusText }

    // return res.json();

    return await fetchWrapper.put(`auctions/6a5011a1-fe1f-47df-9a32-b5346b289391`, data)
}


export async function createAuction(data: FieldValues) {
    return await fetchWrapper.post('auctions', data)
}


export async function updateAuction(id: string, data: FieldValues) {
    const res = await fetchWrapper.put(`auctions/${id}`, data)
    revalidatePath(`/auctions/${id}`);
    return res;
}

export async function getDetailView(id: string): Promise<Auction> {

    return await fetchWrapper.get(`auctions/${id}`)
}

export async function deleteAuction(id: string) {

    return await fetchWrapper.del(`auctions/${id}`)
}